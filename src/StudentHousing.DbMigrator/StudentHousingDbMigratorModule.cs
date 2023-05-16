using StudentHousing.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace StudentHousing.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(StudentHousingEntityFrameworkCoreModule),
        typeof(StudentHousingApplicationContractsModule)
        )]
    public class StudentHousingDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
