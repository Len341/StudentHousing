using StudentHousing.Attachments;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace StudentHousing.Housing
{
    public class PropertyDto: AuditedEntityDto<Guid>
    {
        public string Description { get; set; }
        public List<RegistrationNumberDto> RegistrationNumbers { get; set; }
        public List<AttachmentDto> Attachments { get; set; }

        //TODO: Pin properties on map
    }
}
