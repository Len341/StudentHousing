using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentHousing.Housing;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Identity;

namespace StudentHousing.Web.Pages.Housing
{
    public class PropertyEnquiryModalModel : PageModel
    {
        private readonly IHousingAppService _housingAppService;
        private readonly IIdentityUserRepository _identityUserRepository;

        public PropertyEnquiryModalModel(
            IHousingAppService housingAppService,
            IIdentityUserRepository identityUserRepository)
        {
            _housingAppService = housingAppService;
            _identityUserRepository = identityUserRepository;
        }

        [BindProperty(SupportsGet = true)]
        public Guid? StudentId { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid? PropertyId { get; set; }
        public string PropertyName { get; set; }
        [BindProperty]
        public PropertyEnquiryDto PropertyEnquiry { get; set; }
        public async Task OnGetAsync()
        {
            if (!StudentId.HasValue) throw new UserFriendlyException("No Student Id provided");
            if (!PropertyId.HasValue) throw new UserFriendlyException("No Property Id provided");
            var property = await _housingAppService.GetAsync(PropertyId.Value);
            PropertyName = property.Name;
            var student = await _identityUserRepository.GetAsync(StudentId.Value);
            PropertyEnquiry = new PropertyEnquiryDto();
            PropertyEnquiry.StudentEmail = student.Email;
            PropertyEnquiry.StudentName = $"{student.Name} {student.Surname}";
            PropertyEnquiry.StudentNumber = student.PhoneNumber;
            PropertyEnquiry.PropertyId = PropertyId.Value;
        }
        public async Task OnPostAsync()
        {
            if (!StudentId.HasValue) throw new UserFriendlyException("No Student Id provided");
            if (!PropertyId.HasValue) throw new UserFriendlyException("No Property Id provided");

            await _housingAppService.CreatePropertyEnquiryAsync(PropertyEnquiry);
        }
    }
}
