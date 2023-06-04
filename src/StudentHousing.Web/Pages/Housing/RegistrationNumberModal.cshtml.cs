using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentHousing.Housing;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace StudentHousing.Web.Pages.Housing
{
    public class RegistrationNumberModalModel : PageModel
    {
        private readonly IHousingAppService _housingAppService;
        public RegistrationNumberModalModel(IHousingAppService housingAppService)
        {
            _housingAppService = housingAppService;
        }
        [BindProperty(SupportsGet = true)]
        public Guid? PropertyId { get; set; }
        [BindProperty]
        public string RegistrationNumber { get; set; }
        [BindProperty]
        public string PropertyName { get; set; }
        public async Task OnGetAsync()
        {
            if (!PropertyId.HasValue)
            {
                throw new UserFriendlyException("Property Id Required");
            }
            else
            {
                PropertyName = (await _housingAppService.GetAsync(PropertyId.Value)).Name;
            }
        }
        public async Task OnPostAsync()
        {
            if (PropertyId.HasValue)
            {
                if (!string.IsNullOrEmpty(RegistrationNumber))
                {
                    RegistrationNumberDto registrationNumber = new RegistrationNumberDto();
                    registrationNumber.Number = RegistrationNumber;
                    var property = await _housingAppService.GetAsync(PropertyId.Value);
                    property.RegistrationNumbers.Add(registrationNumber);
                    await _housingAppService.UpdateAsync(property);
                }
            }
            else
            {
                throw new UserFriendlyException("Property Id Required");
            }
        }
    }
}
