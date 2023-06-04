using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NUglify.Helpers;
using StudentHousing.Attachments;
using StudentHousing.Housing;
using StudentHousing.Location;
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
        private readonly ILocationAppService _locationAppService;
        private readonly IConfiguration _configuration;
        public PropertyModalModel(
            IHousingAppService housingAppService,
            ILocationAppService locationAppService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _locationAppService = locationAppService;
            _housingAppService = housingAppService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid? PropertyId { get; set; }
        [BindProperty]
        public PropertyDto Property { get; set; }
        [BindProperty]
        public string DistanceInKm { get; set; }
        public async Task OnGetAsync()
        {
            if (CurrentUser.IsAuthenticated)
            {
                if (PropertyId.HasValue)
                {
                    //update
                    Property = await _housingAppService.GetAsync(PropertyId.Value);
                    if (Property.DistanceFromUniversity.HasValue)
                    {
                        DistanceInKm = String.Format("{0:.##}", (double)Property.DistanceFromUniversity.Value/1000);
                    }
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

                Property.Attachments = attachments;
                if (string.IsNullOrEmpty(Property.PropertyAddress))
                {
                    Property.PropertyLocationCoords = string.Empty;
                }
                else
                {
                    Property.PropertyLocationCoords = await _locationAppService.GetCoordsFromAddress(Property.PropertyAddress);
                }

                if (!string.IsNullOrEmpty(Property.PropertyLocationCoords))
                {
                    //coords exist
                    Property.DistanceFromUniversity = await _locationAppService.GetDistanceInMetersAsync(
                        _configuration["UniversityCoords"],
                        Property.PropertyLocationCoords
                    );
                }

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
