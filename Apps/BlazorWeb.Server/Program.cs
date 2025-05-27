using BlazorWeb.Server;
using BlazorWeb.Server.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UsePipeline();

app.Run();