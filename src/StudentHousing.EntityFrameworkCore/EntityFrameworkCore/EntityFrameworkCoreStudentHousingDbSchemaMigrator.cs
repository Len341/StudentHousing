using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentHousing.Data;
using Volo.Abp.DependencyInjection;

namespace StudentHousing.EntityFrameworkCore
{
    public class EntityFrameworkCoreStudentHousingDbSchemaMigrator
        : IStudentHousingDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreStudentHousingDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the StudentHousingDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<StudentHousingDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
