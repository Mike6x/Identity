using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web.Resource;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Resource_Server_1.Configurations;
using Resource_Server_1.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerConfig(builder.Configuration);

// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//     })
//     .AddJwtBearer(options =>
//     {
//         // base-address of Auth Server
//         options.Authority = "https://localhost:7000/";
//
//         // name of the API resource
//         options.Audience = "Resource_Server_2";
//
//         options.RequireHttpsMetadata = false;
//
//         // Check preferred_username claim exists in the token. If it exists, .NET Core framework sets it to currently logged-in user name i-e User.Identity.Name
//         options.TokenValidationParameters.NameClaimType = "preferred_username";
//         options.TokenValidationParameters.RoleClaimType = System.Security.Claims.ClaimTypes.Role;// "role";
//     })
//     ;
// builder.Services.AddAuthorization();

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:7000/");
        options.AddAudiences("resource_server_1");

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

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

