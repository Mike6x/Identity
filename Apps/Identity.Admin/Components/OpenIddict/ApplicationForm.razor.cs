using Client.Infrastructure.Api;
using Identity.Admin.Components.OpenIddict.ViewModels;
using Identity.Shared.Helpers;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class ApplicationForm : ComponentBase
    {      
        [CascadingParameter]
        public required ApplicationDto Application { get; set; }
        
        [Parameter]
        public required IDialogService Dialog { get; set; }

        [Parameter]
        public ISnackbar? SnackBar { get; set; }

        private readonly List<SwitchItemViewModel> _endPointPermissions =
        [
            new(Permissions.Endpoints.Authorization, false),
            new(Permissions.Endpoints.DeviceAuthorization, false),
            new(Permissions.Endpoints.Introspection, false),
            new(Permissions.Endpoints.EndSession, false),
            new(Permissions.Endpoints.Revocation, false),
            new(Permissions.Endpoints.Token, false)
        ];

        private readonly List<SwitchItemViewModel> _grantTypePermissions =
        [
            new(Permissions.GrantTypes.AuthorizationCode, false),
            new(Permissions.GrantTypes.ClientCredentials, false),
            new(Permissions.GrantTypes.Implicit, false),
            new(Permissions.GrantTypes.Password, false),
            new(Permissions.GrantTypes.RefreshToken, false),
            new(Permissions.GrantTypes.DeviceCode, false)
        ];

        private readonly List<SwitchItemViewModel> _responseTypePermissions =
        [
            new(Permissions.ResponseTypes.Code, false),
            new(Permissions.ResponseTypes.CodeIdToken, false),
            new(Permissions.ResponseTypes.CodeIdTokenToken, false),
            new(Permissions.ResponseTypes.CodeToken, false)
        ];

        private readonly List<SwitchItemViewModel> _scopePermissions =
        [
            new(Permissions.Scopes.Address, false),
            new(Permissions.Scopes.Email, false),
            new(Permissions.Scopes.Phone, false),
            new(Permissions.Scopes.Profile, false),
            new(Permissions.Scopes.Roles, false)
        ];

        private readonly List<SwitchItemViewModel> _requirements =
            [new(Requirements.Features.ProofKeyForCodeExchange, false)];
        
        protected override void OnParametersSet()
        {
            AddCustomScopes();
            InitializePermissionState(_endPointPermissions);
            InitializePermissionState(_grantTypePermissions);
            InitializePermissionState(_responseTypePermissions);
            InitializePermissionState(_scopePermissions);
            InitializeRequirementState(_requirements);
            return;

            //While editing an application back, we need to add any custom scope to the scopePermissions that was previously added 
            void AddCustomScopes()
            {
                if (Application.Permissions == null) return;
                
                var scopes = Application.Permissions.Where(p => p.StartsWith("scp:"));
                foreach (var scope in scopes)
                {
                    if (_scopePermissions.Any(sp => sp.ItemValue.Equals(scope)))
                    {
                        continue;
                    }
                    _scopePermissions.Add(new SwitchItemViewModel(scope, true));
                }
            }
        }

        private void InitializePermissionState(List<SwitchItemViewModel> permissions)
        {
            foreach (var item in permissions)
            {
                if (Application.Permissions != null) 
                    item.IsSelected = Application.Permissions.Contains(item.ItemValue);
            }
        }

        private void InitializeRequirementState(List<SwitchItemViewModel> requirements)
        {
            foreach (var item in requirements)
            {
                if (Application.Requirements != null)
                    item.IsSelected = Application.Requirements.Contains(item.ItemValue);
            }
        }

        /// <summary>
        /// Add a new scope the application permissions list
        /// </summary>
        /// <returns></returns>
        private async Task AddScope()
        {
            var parameters = new DialogParameters();
            var dialog = await Dialog.ShowAsync<AddScopeDialog>("Add New Scope", parameters, new DialogOptions { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if (result != null && !result.Canceled && result.Data is ScopeDto customScope)
            {
                if(Application.Permissions != null && Application.Permissions.Contains(customScope.Name))
                {
                    SnackBar?.Add($"Selected scope {customScope.DisplayName} already exists in application permission.", Severity.Error);
                    return;
                }
                var scopeSwitchItem = new SwitchItemViewModel(customScope.DisplayName, $"scp:{customScope.Name}", false);
                _scopePermissions.Add(scopeSwitchItem);
                TogglePermission(scopeSwitchItem);
            }
        }

        private async Task AddRedirectUri()
        {
            var parameters = new DialogParameters { { "ExistingUris", Application.RedirectUris } };
            
            var dialog = await Dialog.ShowAsync<AddUriComponent>("Add New Uri", parameters, new DialogOptions { MaxWidth = MaxWidth.Large, CloseButton = true }) ;
            var result = await dialog.Result;
            if (result != null && !result.Canceled && result.Data is Uri uriToAdd)
            {
                Application.RedirectUris?.Add(uriToAdd);
            }
        }

        private void RemoveRedirectUri(Uri uri)
        {
            if (Application.RedirectUris != null && Application.RedirectUris.Contains(uri))
            {
                Application.RedirectUris.Remove(uri);
            }
        }

        private async Task AddPostLogoutRedirectUri()
        {
            var parameters = new DialogParameters { { "ExistingUris", Application.PostLogoutRedirectUris } };
            
            var dialog = await Dialog.ShowAsync<AddUriComponent>("Add New Uri", parameters, new DialogOptions { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if(result != null && !result.Canceled && result.Data is Uri uriToAdd)
            {
                Application.PostLogoutRedirectUris?.Add(uriToAdd);
            }            
        }

        private void RemovePostLogoutRedirectUri(Uri uri)
        {
            if (Application.PostLogoutRedirectUris != null && Application.PostLogoutRedirectUris.Contains(uri))
            {
                Application.PostLogoutRedirectUris.Remove(uri);
            }
        }

        private void TogglePermission(SwitchItemViewModel permission)
        {
            if (Application.Permissions != null) ToggleSwitch(permission, Application.Permissions);
        }

        private void ToggleRequirement(SwitchItemViewModel requirement)
        {
            if (Application.Requirements != null) ToggleSwitch(requirement, Application.Requirements);
        }

        private void ToggleSwitch(SwitchItemViewModel item, ICollection<string> targetCollection)
        {
            if (!targetCollection.Contains(item.ItemValue))
            {
                targetCollection.Add(item.ItemValue);
            }
            else if (targetCollection.Contains(item.ItemValue))
            {
                targetCollection.Remove(item.ItemValue);
            }
            item.IsSelected = !item.IsSelected;
        }


        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        private bool _isPasswordVisible;
        private InputType _passwordInputFieldType = InputType.Password;

        private void OnTogglePasswordVisibility()
        {
            if (_isPasswordVisible)
            {
                _isPasswordVisible = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInputFieldType = InputType.Password;
            }
            else
            {
                _isPasswordVisible = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInputFieldType = InputType.Text;
            }
        }

        private async Task AddSetting()
        {
            var parameters = new DialogParameters();
            List<string> configuredSettingNames = [];
            if (Application.Settings != null)
            {
                foreach (var setting in Application.Settings)
                {
                    configuredSettingNames.Add(TokenLifeTimesHelper.GetNameFromValue(setting.Key));
                }

                parameters.Add("ConfiguredSettings", configuredSettingNames);
                var dialog = await Dialog.ShowAsync<ApplicationSettings>("Add New Setting", parameters,
                    new DialogOptions { MaxWidth = MaxWidth.Large, CloseButton = true });
                var result = await dialog.Result;
                if (result is { Canceled: false, Data: KeyValuePair<string, string> settingToAdd })
                {
                    Application.Settings.Add(settingToAdd.Key, settingToAdd.Value);
                }
            }
        }

        private void RemoveSetting(KeyValuePair<string, string> settingToRemove)
        {
            Application.Settings?.Remove(settingToRemove.Key);
        }
    }
}
