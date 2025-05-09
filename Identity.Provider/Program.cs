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