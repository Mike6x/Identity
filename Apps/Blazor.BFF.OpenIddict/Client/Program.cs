using Blazor.BFF.OpenIddict.Client;
using Blazor.BFF.OpenIddict.Client.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Blazor.BFF.OpenIddict.Client.Configurations;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.TryAddSingleton<AuthenticationStateProvider, HostAuthenticationStateProvider>();
builder.Services.TryAddSingleton(sp => (HostAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddTransient<AuthorizedHandler>();

builder.Services.RegisterHttpClient(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();
