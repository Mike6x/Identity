using Identity.Core.Features.Authentication;
using Identity.Core.Features.Client;
using Identity.Core.Features.Scope;
using Identity.Infrastructure.Data.Repository;
using Identity.Infrastructure.Services.Authentication;
using Identity.Infrastructure.Services.Client;
using Identity.Infrastructure.Services.Email;
using Identity.Infrastructure.Services.Logging;
using Identity.Infrastructure.Services.Scope;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Services;

public static class InfraServicesRegister
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
                
        // services.AddScoped<ICurrentUser, CurrentUser>();
        // services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        // services.AddTransient<IUserService, UserService>();
        // services.AddTransient<IRoleService, RoleService>();
        
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IScopeService, ScopeService>();
        services.AddTransient<IApplicationService, ApplicationService>();
        
        // services.AddTransient<IAuthorizationService, OpenIdDictService>();

        
        // services.AddTransient<IPermissionService, PermissionService>()
        //builder.Services.AddSingleton<IEmailSender<AppUser>, IdentityNoOpEmailSender>();
        // builder.Services.AddTransient<AuthorizationService>();
        // services.AddTransient<ClientSeeder>();
        // services.AddScoped<IEmailService, EmailService>()
        
        services.AddSingleton<IEmailSender, EmailSenderService>();
        
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(c => c.LoginPath = "/Account/Login");

        services.AddScoped<ClientAppRepository>();
        services.AddScoped<ScopesRepository>();
        
        services.AddSingleton<LoggerService>();
        return services;
    }
}