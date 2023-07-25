using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlightService.Data
{
    public static class DbPreparer
    {
        public static ILogger Logger { get; set; }

        public static void PrepareDb(IApplicationBuilder app)
        {
            using IServiceScope srvScope = app.ApplicationServices.CreateScope();
            var ctx = srvScope.ServiceProvider.GetService<AppDbContext>();
            if (ctx != null)
            { 
                Logger.LogInformation("Applying migrations");
                try {
                    ctx.Database.Migrate();
                }
                catch (Exception e) {
                    Logger.LogError($"Could not apply migrations: {e.Message}");
                }
            }
        }
    }
}