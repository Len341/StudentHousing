using System;
using System.Collections.Generic;
using System.Text;
using StudentHousing.Localization;
using Volo.Abp.Application.Services;

namespace StudentHousing
{
    /* Inherit your application services from this class.
     */
    public abstract class StudentHousingAppService : ApplicationService
    {
        protected StudentHousingAppService()
        {
            LocalizationResource = typeof(StudentHousingResource);
        }
    }
}
