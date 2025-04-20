using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Identity.BlazorWasm.Infra;

public class LoginService(IHttpClientFactory httpClientFactory) : ILoginService
{
    // Bugfix
    // ConstructorContainsNullParameterNames, System.Net.Http.FormUrlEncodedContent System.NotSupportedException: ConstructorContainsNullParameterNames, System.Net.Http.FormUrlEncodedContent
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Dictionary<string, string>))]
    private readonly Dictionary<string, string> _loginRequest = new()
    {
        { "grant_type", "password" },
        { "client_id", "blazorwasm-oidc-application" },
        { "client_secret", "388D45FA-B36B-4988-BA59-B187D329C206" },
        { "scope", "offline_access" }
    };

    public async Task<LoginResult?> LoginUserAsync(string userName, string password)
    {
        _loginRequest.Remove("username");
        _loginRequest.Remove("password");
        _loginRequest.Add("username", userName);
        _loginRequest.Add("password", password);
        var httpClient = httpClientFactory.CreateClient("AuthorityHttpClient");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        var formUrlEncodedContent = new FormUrlEncodedContent(_loginRequest);
        var response = await httpClient.PostAsync("connect/token", formUrlEncodedContent);
        if (response.StatusCode != HttpStatusCode.OK) return null;
        return await response.Content.ReadFromJsonAsync<LoginResult>();
    }
}