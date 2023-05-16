using StudentHousing.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace StudentHousing.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class StudentHousingPageModel : AbpPageModel
    {
        protected StudentHousingPageModel()
        {
            LocalizationResourceType = typeof(StudentHousingResource);
        }
    }
}