using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace StudentHousing.Data
{
    /* This is used if database provider does't define
     * IStudentHousingDbSchemaMigrator implementation.
     */
    public class NullStudentHousingDbSchemaMigrator : IStudentHousingDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}