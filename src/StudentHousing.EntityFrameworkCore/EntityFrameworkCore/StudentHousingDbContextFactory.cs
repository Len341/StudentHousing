using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StudentHousing.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class StudentHousingDbContextFactory : IDesignTimeDbContextFactory<StudentHousingDbContext>
    {
        public StudentHousingDbContext CreateDbContext(string[] args)
        {
            StudentHousingEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<StudentHousingDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new StudentHousingDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../StudentHousing.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
