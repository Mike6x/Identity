using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components.OpenIddict
{
    public partial class ScopeForm : ComponentBase
    {
        [CascadingParameter]
        public required ScopeDto Scope { get; set; }

        [Parameter]
        public required IDialogService Dialog { get; set; }

        private async Task AddScopeResource()
        {
            var parameters = new DialogParameters { { "ExistingResources", Scope.Resources } };

            var dialog = await Dialog.ShowAsync<AddResourceDialog>("Add Resource", parameters, new DialogOptions() { MaxWidth = MaxWidth.Large, CloseButton = true });
            var result = await dialog.Result;
            if (result is { Canceled: false, Data: string resource })
            {
                Scope.Resources?.Add(resource);
            }
        }


        private void RemoveScopeResource(string scope)
        {
            if(Scope.Resources != null && Scope.Resources.Contains(scope))
            {
                Scope.Resources.Remove(scope);
            }
        }
    }
}
