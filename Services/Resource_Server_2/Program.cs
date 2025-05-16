using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_2;
using Resource_Server_2.Configurations;
using Resource_Server_2.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Get the security configuration constants
var securityConfig = builder.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>() ??
                     throw new NullReferenceException("SecurityConfig is null");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add OpenIddict validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer(securityConfig.Issuer);
        options.AddAudiences(securityConfig.Audience);

        options.AddEncryptionKey(new SymmetricSecurityKey(Convert.FromBase64String(securityConfig.Key)));

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

// Add authentication and authorization
builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Constants.AuthPolicy,
        policy => policy.RequireRole("Editor"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.MapScalarApiReference();
    app.MapScalarApiReference(options => options
        .WithPreferredScheme("OAuth2")
        .AddAuthorizationCodeFlow("OAuth2", flow =>
        {
            flow.ClientId = "mvc-client";
            flow.ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C205";
            flow.Pkce = Pkce.Sha256;
            flow.SelectedScopes = ["profile", "email", "api"];
        }));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapResourceEndpoints();
app.UseHttpsRedirection();

app.Run();