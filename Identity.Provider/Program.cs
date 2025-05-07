using Identity.Provider.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddRazorPages();

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthMiddlewarePipeline();

app.Run();

// await app.StartAsync();
//
// await app.GenerateClients();
//
// await app.WaitForShutdownAsync();

// var builder = WebApplication.CreateBuilder(args);
//
// builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//     .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
// builder.Services.AddRazorPages()
//     .AddMicrosoftIdentityUI();
//
// var app = builder.Build();
//
//
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
//
// app.UseRouting();
//
// app.UseAuthorization();
//
// app.MapStaticAssets();
// app.MapRazorPages()
//     .WithStaticAssets();
// app.MapControllers();
//
// app.Run();