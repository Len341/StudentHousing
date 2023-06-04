using StudentHousing.Attachments;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace StudentHousing.Housing
{
    public class PropertyDto: AuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MonthlyPrice { get; set; }
        public List<RegistrationNumberDto> RegistrationNumbers { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
        public string PropertyLocationCoords { get; set; }
        public string PropertyAddress { get; set; }
        public int? DistanceFromUniversity { get; set; }
    }
}
