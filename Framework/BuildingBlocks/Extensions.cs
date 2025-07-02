using BuildingBlocks.Caching;
using BuildingBlocks.DataIO;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Jobs;
using BuildingBlocks.Mail;
using BuildingBlocks.Origin;
using BuildingBlocks.Storage;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks;

public static class Extensions
{
    public static IServiceCollection AddBlockServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();
        
        services.ConfigureCaching(configuration);
        
        services.ConfigureDataImportExport();
        
        services.ConfigureJobs(configuration);
        
        services.ConfigureMailing();
        
        services.ConfigureFileStorage();
        
        services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));
        
        // Register validators
        // var assemblies = new[]
        // {
        //     typeof(AppBlocks).Assembly
        // };
        // services.AddValidatorsFromAssemblies(assemblies);
        
        return services;
    }
    
    public static WebApplication UseBlockServices(this WebApplication app)
    {
        
        app.UseExceptionHandler(options => { });
        
        app.UseJobDashboard(app.Configuration);
        
        app.UseFileStorage();
        
        return app;
    }
    
}