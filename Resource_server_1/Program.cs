using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_1;
using Resource_Server_1.Configurations;
using Resource_Server_1.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Get the security configuration constants
var securityConfig = builder.Configuration.GetSection("SecurityConfig").Get<SecurityConfig>() ??
                     throw new NullReferenceException("SecurityConfig is null");

builder.Services.AddOpenApi();

builder.Services.AddSwaggerConfig(builder.Configuration);

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

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.MapResourceEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

