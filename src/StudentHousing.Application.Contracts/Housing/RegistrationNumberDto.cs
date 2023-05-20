using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace StudentHousing.Housing
{
    public class RegistrationNumberDto : EntityDto<Guid>
    {
        public string Number { get; set; }
    }
}
