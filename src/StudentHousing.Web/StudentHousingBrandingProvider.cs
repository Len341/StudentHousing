using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace StudentHousing.Web
{
    [Dependency(ReplaceServices = true)]
    public class StudentHousingBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "StudentHousing";
    }
}
