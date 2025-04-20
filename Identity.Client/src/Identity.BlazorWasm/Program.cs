using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Identity.BlazorWasm;
using Identity.BlazorWasm.Infra;
using OpenIddict.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

var serverBaseAddress = builder.Configuration["AuthorityUrl"] ?? "http://localhost:7000";

builder.Services.AddTransient<CustomAuthenticationHandler>();
builder.Services.AddHttpClient("AuthorityHttpClient", client => client.BaseAddress = new Uri(serverBaseAddress))
    .AddHttpMessageHandler<CustomAuthenticationHandler>();

builder.Services.AddOpenIddict().AddClient(options =>
{
    options.AllowPasswordFlow().AllowRefreshTokenFlow();
    options.AddRegistration(
        new OpenIddictClientRegistration { Issuer = new Uri(serverBaseAddress, UriKind.Absolute) });
});

builder.Services.RegisterServices();

await builder.Build().RunAsync();