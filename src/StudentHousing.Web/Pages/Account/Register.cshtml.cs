using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StudentHousing.Roles;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Settings;
using Volo.Abp.Account.Web.Pages.Account;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Validation;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace StudentHousing.Web.Pages.Account;

public class RegisterModel : AccountPageModel
{
    private readonly IIdentityRoleRepository _roleRepo;
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [BindProperty]
    public PostInput Input { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsExternalLogin { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ExternalLoginAuthSchema { get; set; }

    public RegisterModel(
        IAccountAppService accountAppService,
        IIdentityRoleRepository identityRolerepo)
    {
        _roleRepo = identityRolerepo;
        AccountAppService = accountAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        await CheckSelfRegistrationAsync();
        await TrySetEmailAsync();
        return Page();
    }

    private async Task TrySetEmailAsync()
    {
        if (IsExternalLogin)
        {
            var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return;
            }

            if (!externalLoginInfo.Principal.Identities.Any())
            {
                return;
            }

            var identity = externalLoginInfo.Principal.Identities.First();
            var emailClaim = identity.FindFirst(ClaimTypes.Email);
            var nameClaim = identity.FindFirst(ClaimTypes.Name);
            var surnameClaim = identity.FindFirst(ClaimTypes.Surname);

            if (emailClaim == null)
            {
                return;
            }

            Input = new PostInput { EmailAddress = emailClaim.Value };
        }
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            string Role = Request.Form["registerType"];
            await CheckSelfRegistrationAsync();

            await RegisterLocalUserAsync(Role);
            

            return Redirect(ReturnUrl ?? "~/"); //TODO: How to ensure safety? IdentityServer requires it however it should be checked somehow!
        }
        catch (BusinessException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }
    }

    protected virtual async Task RegisterLocalUserAsync(string role)
    {
        try
        {
            ValidateModel();
        }catch(Exception ex)
        {
            throw;
        }
        var userDto = await AccountAppService.RegisterAsync(
            new RegisterDto
            {
                AppName = "MVC",
                EmailAddress = Input.EmailAddress,
                Password = Input.Password,
                UserName = Input.UserName
            }
        );

        var user = await UserManager.GetByIdAsync(userDto.Id);
        user.Name = Input.Firstname ?? string.Empty;
        user.Surname = Input.Surname ?? string.Empty;
        await UserManager.UpdateAsync(user);
        user = await UserManager.GetByIdAsync(userDto.Id);

        if (role != null && role.ToLower() == RoleNames.Student.ToLower())
        {
            await UserManager.SetRolesAsync(user, new[] { RoleNames.Student });
        }else if (role != null && role.ToLower() == RoleNames.Landlord.ToLower())
        {
            await UserManager.SetRolesAsync(user, new[] { RoleNames.Landlord });
        }
        await SignInManager.SignInAsync(user, isPersistent: true);
    }

    protected virtual async Task<IdentityUser> RegisterExternalUserAsync(ExternalLoginInfo externalLoginInfo, PostInput input)
    {
        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), input.EmailAddress, input.EmailAddress, CurrentTenant.Id);
        user.Name = input.Firstname;
        user.Surname = input.Surname;

        (await UserManager.CreateAsync(user)).CheckErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckErrors();

        var userLoginAlreadyExists = user.Logins.Any(x =>
            x.TenantId == user.TenantId &&
            x.LoginProvider == externalLoginInfo.LoginProvider &&
            x.ProviderKey == externalLoginInfo.ProviderKey);

        if (!userLoginAlreadyExists)
        {
            (await UserManager.AddLoginAsync(user, new UserLoginInfo(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                externalLoginInfo.ProviderDisplayName
            ))).CheckErrors();
        }

        await SignInManager.SignInAsync(user, isPersistent: true);
        return user;
    }

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled) ||
            !await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
        }
    }

    public class PostInput
    {
        [Required]
        [MaxLength(50)]
        public string Firstname { get; set; }
        [Required]
        [MaxLength(50)]
        public string Surname { get; set; }
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string EmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string Password { get; set; }
    }
}