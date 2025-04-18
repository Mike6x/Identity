using Identity.API.Configurations;
using Identity.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddAuthServices(builder.Configuration);

builder
    .ConfigureReverseProxySupport();

var app = builder.Build();

app.UseAuthMiddlewarePipeline();

await app.StartAsync();

await app.GenerateClients();

await app.WaitForShutdownAsync();
