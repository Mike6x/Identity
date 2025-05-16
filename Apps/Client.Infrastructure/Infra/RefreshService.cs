using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.String;
using static Client.Infrastructure.Infra.AuthRefreshMessage;

namespace Client.Infrastructure.Infra;

public class RefreshService(
    IHttpClientFactory clientFactory,
    IJwtTokenService jwtTokenService,
    ILogoutService? logoutService = null)
{
    // Bugfix
    // ConstructorContainsNullParameterNames, System.Net.Http.FormUrlEncodedContent System.NotSupportedException: ConstructorContainsNullParameterNames, System.Net.Http.FormUrlEncodedContent
    [DynamicDependency(DynamicallyAccessedMemberTypes.PublicConstructors, typeof(Dictionary<string, string>))]
    private readonly Dictionary<string, string> _refreshRequestModel = new()
    {
        { "grant_type", "refresh_token" },
        { "client_id", "blazorwasm-oidc-application" },
        { "client_secret", "388D45FA-B36B-4988-BA59-B187D329C206" },
    };

    
    public async Task<AuthRefreshResult> RefreshAsync(string? accessToken, string? refreshToken)
    {
        if (IsNullOrWhiteSpace(accessToken) ) { return new AuthRefreshResult(InputAccessTokenNull ); }
        if (IsNullOrWhiteSpace(refreshToken)) { return new AuthRefreshResult(InputRefreshTokenNull); }

        var httpClient = clientFactory.CreateClient("AuthorityHttpClient");
        
        try
        {
            _refreshRequestModel.Add("refresh_token", refreshToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var formUrlEncodedContent = new FormUrlEncodedContent(_refreshRequestModel);
            var response = await httpClient.PostAsync("connect/token", formUrlEncodedContent);
            if (response.StatusCode != HttpStatusCode.OK) return new AuthRefreshResult(HttpStatusCodeNok);
            
            if (response is { IsSuccessStatusCode: true })
            {
                var result = await response.Content.ReadFromJsonAsync<RefreshResult>();
                if (result == null) return new AuthRefreshResult(ResponseContentNull);
                
                if (IsNullOrWhiteSpace(result.access_token)) return new AuthRefreshResult(AccessTokenNull);
                if (IsNullOrWhiteSpace(result.refresh_token)) return new AuthRefreshResult(RefreshTokenNull);
                if (!jwtTokenService.IsValid(result.access_token)) return new AuthRefreshResult(AccessTokenInvalid);

                await jwtTokenService.SaveAuthTokensAsync(result.access_token, result.refresh_token);

                return new AuthRefreshResult(result.access_token, result.refresh_token, result.expires_in);
            }
        }
        catch (Exception ex)
        {
            if (logoutService != null) await logoutService.LogoutAsync();
            return new AuthRefreshResult(ExceptionThrown);
        }

        if (logoutService != null) await logoutService.LogoutAsync();
        return new AuthRefreshResult(SomethingWentWrong);
    }
}