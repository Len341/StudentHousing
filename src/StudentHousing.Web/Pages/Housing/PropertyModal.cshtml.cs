using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUglify.Helpers;
using StudentHousing.Attachments;
using StudentHousing.Housing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace StudentHousing.Web.Pages.Housing
{
    public class PropertyModalModel : AbpPageModel
    {
        private readonly IHousingAppService _housingAppService;
        public PropertyModalModel(IHousingAppService housingAppService)
        {
            _housingAppService = housingAppService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid? PropertyId { get; set; }
        [BindProperty]
        public PropertyDto Property { get; set; }
        public async Task OnGetAsync()
        {
            if (CurrentUser.IsAuthenticated)
            {
                if (PropertyId.HasValue)
                {
                    //update
                    Property = await _housingAppService.GetAsync(PropertyId.Value);
                }
                else
                {
                    //create
                    Property = new PropertyDto();
                }
            }
            else
            {
                throw new UserFriendlyException("Not authorized");
            }
        }
        public async Task OnPostAsync()
        {
            if (CurrentUser.IsAuthenticated)
            {
                var images = Request.Form.Files;
                List<AttachmentDto> attachments = new List<AttachmentDto>();
                if (images.Any())
                {
                    images.ForEach(async image =>
                    {
                        if (image.Length > 0)
                        {
                            string filePath = Path.Combine("wwwroot/PropertyImages", image.FileName);
                            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                image.CopyTo(fileStream);
                                attachments.Add(new AttachmentDto() { Url = filePath.Replace("wwwroot/", ""), AttachmentName = image.Name, FileName = image.FileName });
                            }
                        }
                    });
                }
                Console.WriteLine(attachments);
                Property.Attachments = attachments;
                if (PropertyId.HasValue)
                {
                    //update
                    await _housingAppService.UpdateAsync(Property);
                }
                else
                {
                    //create
                    await _housingAppService.CreateAsync(Property);
                }
            }
            else
            {
                throw new UserFriendlyException("Not authorized");
            }
        }
    }
}
