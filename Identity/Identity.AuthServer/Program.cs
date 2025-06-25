using Identity.AuthServer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddRazorPages();

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseAuthPipeline();

app.Run();
