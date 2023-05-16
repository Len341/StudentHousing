using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace StudentHousing
{
    public class StudentHousingWebTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<StudentHousingWebTestModule>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}