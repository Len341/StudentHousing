using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentHousing.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp;
using Volo.Abp.Account.Settings;
using Volo.Abp.Account.Web;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;
using IdentityUser = Volo.Abp.Identity.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace StudentHousing.Web.Pages.Account;

public class LoginModel : AccountPageModel
{
    private readonly IIdentityUserRepository _userRepo;
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string Role { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; }

    public bool EnableLocalLogin { get; set; }

    //TODO: Why there is an ExternalProviders if only the VisibleExternalProviders is used.
    public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; }
    public IEnumerable<ExternalProviderModel> VisibleExternalProviders => ExternalProviders.Where(x => !String.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => false;
    public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    //Optional IdentityServer services
    //public IIdentityServerInteractionService Interaction { get; set; }
    //public IClientStore ClientStore { get; set; }
    //public IEventService IdentityServerEvents { get; set; }

    protected IAuthenticationSchemeProvider SchemeProvider { get; }
    protected AbpAccountOptions AccountOptions { get; }
    protected IIdentityRoleRepository _roleRepo { get; }
    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public bool ShowCancelButton { get; set; }

    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IIdentityUserRepository identityUserRepository,
        IIdentityRoleRepository identityRoleRepository,
        IOptions<IdentityOptions> identityOptions)
    {
        _userRepo = identityUserRepository;
        _roleRepo = identityRoleRepository;
        SchemeProvider = schemeProvider;
        IdentityOptions = identityOptions;
        AccountOptions = accountOptions.Value;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();

        ExternalProviders = await GetExternalProviders();
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        if (IsExternalLoginOnly)
        {
            return await OnPostExternalLogin(ExternalProviders.First().AuthenticationScheme);
        }

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        await CheckLocalLoginAsync();

        ValidateModel();

        ExternalProviders = await GetExternalProviders();

        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        await ReplaceEmailToUsernameOfInputIfNeeds();

        await IdentityOptions.SetAsync();

        var result = await SignInManager.PasswordSignInAsync(
            LoginInput.UserNameOrEmailAddress,
            LoginInput.Password,
            LoginInput.RememberMe,
            true
        );

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress
        });

        if (result.RequiresTwoFactor)
        {
            return await TwoFactorLoginResultAsync();
        }

        if (result.IsLockedOut)
        {
            Alerts.Warning(L["UserLockedOutMessage"]);
            return Page();
        }

        if (result.IsNotAllowed)
        {
            Alerts.Warning(L["LoginIsNotAllowed"]);
            return Page();
        }

        if (!result.Succeeded)
        {
            Alerts.Danger(L["InvalidUserNameOrPassword"]);
            return Page();
        }

        //TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
                   await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);

        Debug.Assert(user != null, nameof(user) + " != null");
        return RedirectSafely(ReturnUrl, ReturnUrlHash);

    }

    /// <summary>
    /// Override this method to add 2FA for your application.
    /// </summary>
    protected virtual Task<IActionResult> TwoFactorLoginResultAsync()
    {
        throw new NotImplementedException();
    }

    protected virtual async Task<List<ExternalProviderModel>> GetExternalProviders()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();

        return schemes
            .Where(x => x.DisplayName != null || x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
            .Select(x => new ExternalProviderModel
            {
                DisplayName = x.DisplayName,
                AuthenticationScheme = x.Name
            })
            .ToList();
    }

    public virtual async Task<IActionResult> OnPostExternalLogin(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash, Role });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
    {
        if (remoteError != null)
        {
            Logger.LogWarning($"External login callback error: {remoteError}");
            return RedirectToPage("./Login");
        }

        await IdentityOptions.SetAsync();

        var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
        }

        var result = await SignInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider,
            loginInfo.ProviderKey,
            isPersistent: true,
            bypassTwoFactor: true
        );

        if (!result.Succeeded)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = "Login" + result
            });
        }

        if (result.IsLockedOut)
        {
            Logger.LogWarning($"External login callback error: user is locked out!");
            throw new UserFriendlyException("Cannot proceed because user is locked out!");
        }

        if (result.IsNotAllowed)
        {
            Logger.LogWarning($"External login callback error: user is not allowed!");
            throw new UserFriendlyException("Cannot proceed because user is not allowed!");
        }

        if (result.Succeeded)
        {
            return RedirectSafely(returnUrl, returnUrlHash);
        }

        //TODO: Handle other cases for result!

        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        //if (email.IsNullOrWhiteSpace())
        //{
        //    return RedirectToPage("./Register", new
        //    {
        //        IsExternalLogin = true,
        //        ExternalLoginAuthSchema = loginInfo.LoginProvider,
        //        ReturnUrl = returnUrl
        //    });
        //}

        var user = await UserManager.FindByEmailAsync(email == null ? "" : email);
        if (user == null)
        {
            if (string.IsNullOrEmpty(Role))
            {
                throw new UserFriendlyException("Please register as a client or freelancer first before logging in");
            }
            else
            {
                user = await CreateExternalUserAsync(loginInfo);
            }
        }
        else
        {
            if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
            {
                CheckIdentityErrors(await UserManager.AddLoginAsync(user, loginInfo));
            }
        }

        try
        {
            user = await _userRepo.GetAsync(user.Id);
            user.Name = user.UserName.Split("_")[0].Replace(".", " ");
            user.Surname = user.UserName.Split("_")[1].Replace(".", " ");
            user = await _userRepo.UpdateAsync(user);
            await SignInManager.SignInAsync(user, false);
        }
        catch (Exception ex)
        {
            //throw ex;
            Console.WriteLine(ex);
        }
        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = user.Name
        });

        return RedirectSafely(returnUrl, returnUrlHash);
    }

    protected virtual async Task<IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        await IdentityOptions.SetAsync();

        var emailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);

        var user = new IdentityUser(GuidGenerator.Create(),
            $"{info.Principal.FindFirstValue(AbpClaimTypes.Name).Replace(" ", ".")}_{info.Principal.FindFirstValue(AbpClaimTypes.SurName).Replace(" ", ".")}_" +
            $"{(await LazyServiceProvider.LazyGetRequiredService<IIdentityUserRepository>().GetListAsync()).Count}", emailAddress, CurrentTenant.Id);

        user.IsExternal = true;
        var roleId = (await _roleRepo.FindByNormalizedNameAsync(Role?.ToUpper()))?.Id;
        if (roleId != null)
        {
            user.AddRole((Guid)roleId);
        }
        CheckIdentityErrors(await UserManager.CreateAsync(user));
        CheckIdentityErrors(await UserManager.SetEmailAsync(user, emailAddress));
        CheckIdentityErrors(await UserManager.AddLoginAsync(user, info));
        CheckIdentityErrors(await UserManager.AddDefaultRolesAsync(user));

        user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
        user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);
        var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
        if (!phoneNumber.IsNullOrWhiteSpace())
        {
            var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.InvariantCultureIgnoreCase);
            user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
        }
        return user;
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds()
    {
        if (!ValidationHelper.IsValidEmailAddress(LoginInput.UserNameOrEmailAddress))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        if (userByEmail == null)
        {
            return;
        }

        LoginInput.UserNameOrEmailAddress = userByEmail.UserName;
    }

    protected virtual async Task CheckLocalLoginAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["LocalLoginDisabledMessage"]);
        }
    }

    public class LoginInputModel
    {
        [Required]
        [DisplayName("Username or email address")]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }

    public class ExternalProviderModel
    {
        public string DisplayName { get; set; }
        public string AuthenticationScheme { get; set; }
    }
}