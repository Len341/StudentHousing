using StudentHousing.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace StudentHousing.Housing
{
    public class Property : AuditedEntity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RegistrationNumber> RegistrationNumbers { get; set; }
        public List<Attachment> Attachments { get; set; }

        //TODO: Pin properties on map
    }
}
