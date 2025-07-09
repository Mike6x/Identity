using Client.Infrastructure.Api;
using Identity.Admin.Components.OpenIddict.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Applications
{

    public partial class AddApplication : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? AuthState { get; set; }
        [Inject]
        protected IAuthorizationService? AuthService { get; set; }
        
        [Inject]
        public  required IDialogService Dialog { get; set; }
        
        [Inject]
        public NavigationManager? Navigator { get; set; }
        
        [Inject]
        public required IApiClient ApiClient { get; set; } 
        
        private ApplicationDto _model = new();
        
        private bool _loaded;

        private readonly Func<ApplicationPreset, string> _displayStringConverter = ci => ci.ToDisplayString();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            _model.ApplyPreset(ApplicationPreset.AuthorizationCodeFlow);
            
            _loaded = true;
        }

        private void ApplyPreset(ApplicationPreset preset)
        {
            this._model.ApplyPreset(preset);
        }

        private async Task AddApplicationDetailsAsync()
        {
            if(_model.IsConfidentialClient)
            {
                await Dialog.ShowMessageBox("Information", "Store client secret safely as it can't be viewed later.",
                 "Ok", options: new DialogOptions() { FullWidth = true });
            }
           
            
            var request = _model.Adapt<CreateClientCommand>();
        
            await ApiHelper.ExecuteCallGuardedAsync(
                () => ApiClient.CreateApplicationEndpointAsync(request), 
                Toast, 
                successMessage: "New client was created successfully");

            Navigation.NavigateTo("/identity/applications");
            
        }
    }
}
