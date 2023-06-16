using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace StudentHousing.Housing
{
    public class HousingAppService : ApplicationService, IHousingAppService
    {
        private readonly IHousingRepository _housingRepository;
        public HousingAppService(IHousingRepository housingRepository)
        {
            _housingRepository = housingRepository;
        }

        public async Task<PropertyDto> CreateAsync(PropertyDto property)
        {
            var house = await _housingRepository.CreateAsync(ObjectMapper.Map<PropertyDto, Property>(property));
            return ObjectMapper.Map<Property, PropertyDto>(house);
        }

        public async Task<PropertyEnquiryDto> CreatePropertyEnquiryAsync(PropertyEnquiryDto enquiry)
        {
            return ObjectMapper.Map<PropertyEnquiry, PropertyEnquiryDto>
                (await _housingRepository.CreatePropertyEnquiryAsync(ObjectMapper.Map<PropertyEnquiryDto, PropertyEnquiry>(enquiry)));
        }

        public async Task DeleteAsync(Guid id)
        {
            await _housingRepository.DeleteAsync(id);
        }

        public async Task<PropertyDto> GetAsync(Guid id)
        {
            var property = await _housingRepository.GetAsync(id);
            return ObjectMapper.Map<Property, PropertyDto>(property);
        }

        public async Task<List<PropertyDto>> GetListAsync(PropertySearchInput input)
        {
            var property = await _housingRepository.GetListAsync();

            if (!string.IsNullOrEmpty(input.Price?.Trim()))
            {
                int priceFrom = int.Parse(input.Price.Split(',')[0]);
                int priceTo = int.Parse(input.Price.Split(',')[1]);

                property = property.Where(z => z.MonthlyPrice >= priceFrom && z.MonthlyPrice <= priceTo).ToList();
            }

            if (!string.IsNullOrEmpty(input.Distance?.Trim()))
            {
                int distanceFrom = int.Parse(input.Distance.Split(',')[0]);
                int distanceTo = int.Parse(input.Distance.Split(',')[1]);

                property = property.Where(z => ((decimal)z.DistanceFromUniversity / 1000) >= (decimal)distanceFrom &&
                                               ((decimal)z.DistanceFromUniversity / 1000) <= (decimal)distanceTo).ToList();
            }

            return ObjectMapper.Map<List<Property>, List<PropertyDto>>(property);
        }

        public async Task<List<PropertyEnquiryDto>> GetPropertyEnquiryListAsync()
        {
            return ObjectMapper.Map<List<PropertyEnquiry>, List<PropertyEnquiryDto>>(await _housingRepository.GetPropertyEnquiryListAsync());
        }

        public async Task<PropertyDto> UpdateAsync(PropertyDto property)
        {
            var house = await _housingRepository.UpdateAsync(ObjectMapper.Map<PropertyDto, Property>(property));
            return ObjectMapper.Map<Property, PropertyDto>(house);
        }
    }
}
