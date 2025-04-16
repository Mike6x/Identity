using Identity.API.Data;
using Identity.API.EndPoints;
using Identity.API.EndPoints.Identity;
using Identity.API.EndPoints.OpenIDConnect;
using Identity.API.EndPoints.Users;
using Identity.API.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    // options.UseSqlite(connectionString);
    
    // options.UseNpgsql(connectionString);
    // options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
});

builder.Services
    .AutoRegisterFromIdentityAPI();

builder
    .ConfigureIdentity()
    .ConfigureOpenIddict()
    .ConfigureCors()
    .ConfigureSwagger()
    .ConfigureReverseProxySupport()
    ;

builder.Services
    .AddAntiforgery();

builder.Services
    .AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.UseHttpsRedirection();

app.UseSwagger();
app.UseUrlsFromConfig();
app.UseReverseProxySupport();
app.UseOpenIddict();

app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

var apiEnpoints = app
    .MapGroup("api");
apiEnpoints.MapStatusEndpoints();
apiEnpoints.MapUsersEndpoints();
apiEnpoints.MapIdentityEndpoints();

app.MapOpenIdConnectEndpoints();
app.MapExternalCallbackEndpoint();

app.UseVueFallbackSpa();

await app.StartAsync();

await app.GenerateClients();

await app.WaitForShutdownAsync();
