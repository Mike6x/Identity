using OpenIddict.Validation.AspNetCore;
using Resource_Server_1.Configurations;
using Resource_Server_1.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddOpenIdDictConfig(builder.Configuration);

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerService();

var resourceGroup = app.MapGroup("resources").WithTags("resources server 1");
resourceGroup.MapResourceEndpoints();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

