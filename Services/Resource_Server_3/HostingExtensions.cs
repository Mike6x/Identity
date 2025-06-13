using Identity.Shared.Authorization;
using IdentityModel.AspNetCore.OAuth2Introspection;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_3.Configurations;
using Resource_Server_3.Services;

namespace Resource_Server_3;

internal static class HostingExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddCorsPolicy(configuration);

        services.AddSwaggerConfig(configuration);

        services.AddOpenIdDictConfig(configuration);
        
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
       
        //Configure Authentication to use introspection i.e. API will check with OAuth2 introspection endpoint to validate 
        //if request is authenticated.
        // services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
        //     .AddOAuth2Introspection(options =>
        //     {
        //         options.Authority = "http://localhost:7000";
        //         options.ClientId = "api.resource.server";
        //         options.ClientSecret = "api.resource.secret";
        //     });
        
        // var policyNames = new List<string> { AppScopes.CityReadScope, AppScopes.CityWriteScope, AppScopes.WeatherReadScope};
        // services.AddOpenIdAuth(configuration, policyNames);
        
       
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy(AppPolicies.CanManageStudents, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Manager);
            });
            options.AddPolicy(AppPolicies.CanManageCities, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Manager);
            });
            
            options.AddPolicy(AppPolicies.PaidForecast, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            });
            
            // options.AddPolicy(AppScopes.WeatherReadScope, policy =>
            // {
            //     policy.RequireAuthenticatedUser();
            //     policy.RequireClaim(ClaimConstants.Permissions, AppScopes.WeatherReadScope);
            // });
            
            // options.AddPolicy(AppScopes.WeatherReadScope, policy =>
            // {
            //     policy.RequireAuthenticatedUser();
            //     policy.RequireClaim("read-weather", "true");
            // });
        });
           
        services.AddSingleton<ICityService, JsonCityDataService>();
        
        services.AddSingleton<IStudentService, JsonStudentDataService>();
        
        return services;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();            
        }  

        app.UseSwaggerService();

        app.UseRouting();
        app.UseCorsPolicy();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}