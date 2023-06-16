using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace StudentHousing.Housing
{
    public interface IHousingAppService : IApplicationService
    {
        Task<PropertyDto> CreateAsync(PropertyDto property);
        Task<PropertyDto> UpdateAsync(PropertyDto property);
        Task DeleteAsync(Guid id);
        Task<List<PropertyDto>> GetListAsync(PropertySearchInput input);
        Task<PropertyDto> GetAsync(Guid id);

        Task<PropertyEnquiryDto> CreatePropertyEnquiryAsync(PropertyEnquiryDto enquiry);
        Task<List<PropertyEnquiryDto>> GetPropertyEnquiryListAsync();
    }
}
