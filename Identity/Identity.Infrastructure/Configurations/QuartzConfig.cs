using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Identity.Infrastructure.Configurations;

public static class QuartzConfig
{
    
    public static void ConfigureQuartz(IServiceCollection services)
    {
        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
        // services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
    
}