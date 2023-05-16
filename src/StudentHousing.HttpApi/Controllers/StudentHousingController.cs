using StudentHousing.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace StudentHousing.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class StudentHousingController : AbpControllerBase
    {
        protected StudentHousingController()
        {
            LocalizationResource = typeof(StudentHousingResource);
        }
    }
}