// using System.Net;
// using System.Net.Http.Headers;
// using Microsoft.Extensions.Configuration;
// using static System.Console;
// using static System.String;
//
// namespace Client.Infrastructure.Infra;
//
// public class CustomAuthenticationHandler(IConfiguration configuration, IJwtTokenService jwtTokenService, IHttpClientFactory clientFactory) : DelegatingHandler
// {
//     private bool _refreshing;
//
//     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
//         CancellationToken cancellationToken)
//     {
//         var (accessToken, refreshToken) = await jwtTokenService.GetAuthTokensAsync(cancellationToken);
//         var value = configuration["AuthorityUrl"];
//         var isToServer = request.RequestUri?.AbsoluteUri.StartsWith(value ?? throw new InvalidOperationException() ) ?? false;
//
//         if (isToServer && !IsNullOrEmpty(accessToken))
//             request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//
//         var iShouldRefresh = false;
//         HttpResponseMessage? response = null;
//         try
//         {
//             var requestRequestUri = request.RequestUri;
//             if (requestRequestUri != null && requestRequestUri.ToString().EndsWith("connect/token")) request.Headers.Remove("Authorization");
//             response = await base.SendAsync(request, cancellationToken);
//             iShouldRefresh = response.StatusCode == HttpStatusCode.Unauthorized;
//             if (iShouldRefresh == false) return response;
//         }
//         catch (HttpRequestException httpRequestException)
//         {
//             WriteLine(httpRequestException);
//             iShouldRefresh = true;
//         }
//         catch (Exception exception)
//         {
//             WriteLine(exception);
//         }
//
//         if (_refreshing || IsNullOrEmpty(accessToken) || !iShouldRefresh)
//         {
//             WriteLine($"response {response?.StatusCode} - {response?.ReasonPhrase}");
//             return response ?? throw new InvalidOperationException();
//         }
//
//         try
//         {
//             _refreshing = true;
//             var refreshResult = await new RefreshService(clientFactory, jwtTokenService).RefreshAsync(accessToken, refreshToken);
//             if (refreshResult.Failure) return response ?? throw new InvalidOperationException();
//
//             (accessToken, _) = await jwtTokenService.GetAuthTokensAsync(cancellationToken);
//
//             if (jwtTokenService.IsValid(accessToken) && isToServer)
//                 request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//
//             return await base.SendAsync(request, cancellationToken);
//         }
//         finally
//         {
//             _refreshing = false;
//         }
//     }
// }