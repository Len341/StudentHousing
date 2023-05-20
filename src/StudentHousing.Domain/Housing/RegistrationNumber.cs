using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace StudentHousing.Housing
{
    public class RegistrationNumber : Entity<Guid>
    {
        public string Number { get; set; }
    }
}
