using IdentityModel.AspNetCore.OAuth2Introspection;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_3.Configurations;

namespace Resource_Server_3;

internal static class HostingExtensions
{
    // private static IWebHostEnvironment _env;
    
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddControllers();

       services.AddCorsPolicy(configuration);

       services.AddSwaggerConfig(configuration);

       services.AddOpenIdDictConfig(configuration);

       services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
            .AddOAuth2Introspection(options =>
            {
                options.Authority = "http://localhost:7000";
                options.ClientId = "catalog.resource.server";
                options.ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342";
                // options.ClientId = "service-worker";
                // options.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207";
            });

//Configure Authentication to use introspection i.e. API will check with OAuth2 introspection endpoint to validate 
//if request is authenticated.
//services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
//     .AddOAuth2Introspection(options =>
//     {
//         options.Authority = "http://localhost:7000/pauth";
//         options.ClientId = "service-worker";
//         options.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207";
//     });

//builder.Services.AddAuthorization();
//Configure authorization policy that requires read-weather = true claim to read weather data
       services.AddAuthorizationCore(options =>
        {
            //Add a policy to require read-weather claim
            options.AddPolicy(Policies.ReadWeatherDataPolicy, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("read-weather", "true");
            });
        });
        
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