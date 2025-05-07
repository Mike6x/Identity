using Identity.Infrastructure.Data.Repository;
using Identity.Infrastructure.Services;
using Identity.Infrastructure.Services.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Extensions;

public static class InfraServicesRegister
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
                
        //builder.Services.AddSingleton<IEmailSender<AppUser>, IdentityNoOpEmailSender>();
        // builder.Services.AddTransient<AuthorizationService>();
        // services.AddTransient<ClientSeeder>();
        
        services.AddSingleton<IEmailSender, EmailSenderService>();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(c => c.LoginPath = "/Account/Login");

        services.AddScoped<ClientAppRepository>();
        services.AddScoped<ScopesRepository>();
        
        services.AddSingleton<LoggerService>();
        return services;
    }
}