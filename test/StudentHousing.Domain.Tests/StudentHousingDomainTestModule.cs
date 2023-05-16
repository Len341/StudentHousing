using StudentHousing.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace StudentHousing
{
    [DependsOn(
        typeof(StudentHousingEntityFrameworkCoreTestModule)
        )]
    public class StudentHousingDomainTestModule : AbpModule
    {

    }
}