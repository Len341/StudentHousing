using AutoMapper;
using StudentHousing.Attachments;
using StudentHousing.Housing;

namespace StudentHousing
{
    public class StudentHousingApplicationAutoMapperProfile : Profile
    {
        public StudentHousingApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<Attachment, AttachmentDto>().ReverseMap();
            CreateMap<Property, PropertyDto>().ReverseMap();
            CreateMap<RegistrationNumber, RegistrationNumberDto>().ReverseMap();
        }
    }
}
