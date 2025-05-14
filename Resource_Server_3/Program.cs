using IdentityModel.AspNetCore.OAuth2Introspection;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_3.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddOpenIdDictConfig(builder.Configuration);

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)
    .AddOAuth2Introspection(options =>
    {
        options.Authority = "http://localhost:7000/pauth";
        options.ClientId = "catalog.resource.server";
        options.ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342";
        // options.ClientId = "service-worker";
        // options.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207";
    });

//Configure Authentication to use introspection i.e. API will check with OAuth2 introspection endpoint to validate 
//if request is authenticated.
// builder.Services.AddAuthentication(OAuth2IntrospectionDefaults.AuthenticationScheme)
//     .AddOAuth2Introspection(options =>
//     {
//         options.Authority = "http://localhost:7000/pauth";
//         options.ClientId = "service-worker";
//         options.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207";
//     });

//builder.Services.AddAuthorization();
//Configure authorization policy that requires read-weather = true claim to read weather data
builder.Services.AddAuthorizationCore(options =>
{
    //Add a policy to require read-weather claim
    options.AddPolicy(Policies.ReadWeatherDataPolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("read-weather", "true");
    });
});

var app = builder.Build();

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

app.Run();