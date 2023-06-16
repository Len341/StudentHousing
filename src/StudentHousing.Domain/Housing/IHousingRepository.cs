using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace StudentHousing.Housing
{
    public interface IHousingRepository : IRepository
    {
        Task<Property> CreateAsync(Property property);
        Task<Property> UpdateAsync(Property property);
        Task DeleteAsync(Guid id);
        Task<List<Property>> GetListAsync();
        Task<Property> GetAsync(Guid id);

        Task<PropertyEnquiry> CreatePropertyEnquiryAsync(PropertyEnquiry enquiry);
        Task<List<PropertyEnquiry>> GetPropertyEnquiryListAsync();
    }
}
