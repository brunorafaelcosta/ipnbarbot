using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ipnbarbot.Data
{
    public class ApplicationDbContextSeed
    {
        public async Task SeedAsync(ApplicationDbContext context, IHostingEnvironment env,
            ILogger<ApplicationDbContextSeed> logger, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                // if (!context.Users.Any())
                // {
                //     context.Users.Add(new Models.User
                //     {
                //         Name = "random"
                //     });

                //     await context.SaveChangesAsync();
                // }
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                    await SeedAsync(context, env, logger, retryForAvaiability);
                }
            }
        }
    }
}