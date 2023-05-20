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

        public async Task DeleteAsync(Guid id)
        {
            await _housingRepository.DeleteAsync(id);
        }

        public async Task<PropertyDto> GetAsync(Guid id)
        {
            var property = await _housingRepository.GetAsync(id);
            return ObjectMapper.Map<Property, PropertyDto>(property);
        }

        public async Task<List<PropertyDto>> GetListAsync()
        {
            var property = await _housingRepository.GetListAsync();
            return ObjectMapper.Map<List<Property>, List<PropertyDto>>(property);
        }

        public async Task<PropertyDto> UpdateAsync(PropertyDto property)
        {
            var house = await _housingRepository.UpdateAsync(ObjectMapper.Map<PropertyDto, Property>(property));
            return ObjectMapper.Map<Property, PropertyDto>(house);
        }
    }
}
