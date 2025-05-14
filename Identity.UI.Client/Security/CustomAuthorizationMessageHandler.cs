using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Identity.UI.Client.Security;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager) : base(provider, navigationManager)
    {
        ConfigureHandler(authorizedUrls: ["https://localhost:7203/"]); // User of API Resource
    }
}