using Volo.Abp.Modularity;

namespace StudentHousing
{
    [DependsOn(
        typeof(StudentHousingApplicationModule),
        typeof(StudentHousingDomainTestModule)
        )]
    public class StudentHousingApplicationTestModule : AbpModule
    {

    }
}