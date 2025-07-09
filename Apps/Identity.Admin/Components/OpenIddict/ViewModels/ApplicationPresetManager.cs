using Client.Infrastructure.Api;
using OpenIddict.Abstractions;

namespace Identity.Admin.Components.OpenIddict.ViewModels
{
    public enum ApplicationPreset
    {   
        None,
        AuthorizationCodeFlow,
        ClientCredentialsFlow,
        DeviceAuthorizationFlow,
        Introspection
    }

    public static class ApplicationPresetManager
    {        
        public static void ApplyPreset(this ApplicationDto applicationViewModel, ApplicationPreset preset)
        {
            applicationViewModel.Permissions ??= new List<string>();
            applicationViewModel.Requirements  ??= new List<string>();
            
            applicationViewModel.RedirectUris ??= new List<Uri>();
            applicationViewModel.PostLogoutRedirectUris ??= new List<Uri>();
            applicationViewModel.Settings = new Dictionary<string, string>();
            
            
            applicationViewModel.Permissions.Clear();
            applicationViewModel.Requirements.Clear();
            applicationViewModel.ClientType = OpenIddictConstants.ClientTypes.Public;
            applicationViewModel.ConsentType = OpenIddictConstants.ConsentTypes.Explicit;

            switch (preset)
            {
                case ApplicationPreset.AuthorizationCodeFlow:
                    applicationViewModel.Requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.EndSession);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);
                    break;
                case ApplicationPreset.ClientCredentialsFlow:
                    applicationViewModel.ClientType = OpenIddictConstants.ClientTypes.Confidential;
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);                  
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
                    break;
                case ApplicationPreset.DeviceAuthorizationFlow:                   
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Email);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Profile);
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Scopes.Roles);
                    break;
                case ApplicationPreset.Introspection:
                    applicationViewModel.ClientType = OpenIddictConstants.ClientTypes.Confidential;
                    applicationViewModel.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
                    break;
            }           
        }

        public static string ToDisplayString(this ApplicationPreset preset)
        {            
            return preset switch
            {
                ApplicationPreset.AuthorizationCodeFlow => "Authorization Code Flow",
                ApplicationPreset.ClientCredentialsFlow => "Client Credentials Flow",
                ApplicationPreset.DeviceAuthorizationFlow => "Device Authorization Flow",
                ApplicationPreset.Introspection => "Introspection",
                ApplicationPreset.None => "None",
                _ => preset.ToString()
            };
        }
    }
}
