using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace StudentHousing.Housing
{
    public class PropertyEnquiry : AuditedEntity<Guid>
    {
        public Guid PropertyId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentNumber { get; set; }
        public string MessageFromStudent { get; set; }
    }
}
