using BuildingBlocks.Auth;
using BuildingBlocks.Identity.Users.Abstractions;
using Identity.Core.Features.Authentication;
using Identity.Core.Features.Client;
using Identity.Core.Features.Role;
using Identity.Core.Features.Scope;
using Identity.Core.Features.User;
using Identity.Infrastructure.Data.Repository;
using Identity.Infrastructure.Services.Authentication;
using Identity.Infrastructure.Services.Client;
using Identity.Infrastructure.Services.Logging;
using Identity.Infrastructure.Services.Role;
using Identity.Infrastructure.Services.Scope;
using Identity.Infrastructure.Services.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure.Services;

public static class InfraServicesRegister
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CurrentUserMiddleware>();        
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
        
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IScopeService, ScopeService>();
        
        services.AddTransient<IApplicationService, ApplicationService>();
        
        services.AddTransient<IAuthService, AuthService>();
        
        // services.AddTransient<IAuthorizationService, OpenIdDictService>();
        // services.AddTransient<AuthorizationService>();
        // services.AddTransient<IPermissionService, PermissionService>()
        
        services.AddScoped<ClientAppRepository>();
        services.AddScoped<ScopesRepository>();
        
        services.AddSingleton<LoggerService>();
        
        return services;
    }
}