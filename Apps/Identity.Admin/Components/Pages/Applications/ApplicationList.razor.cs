using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
// using Pixel.Identity.Shared.Request;
// using Pixel.Identity.Shared.ViewModels;
// using Pixel.Identity.UI.Client.Services;

namespace Identity.Admin.Components.Pages.Applications
{

    public partial class ApplicationList : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? AuthState { get; set; }

        private readonly ApplicationViewModel _viewModel = new();
        [Inject]
        public IApiClient? ApiClient { get; set; }
        
        protected override async Task OnInitializedAsync()
        {
            var items = await ApiClient.GetApplicationsEndpointAsync().ConfigureAwait(false);
 
        }
        
        private readonly int[] pageSizeOptions = { 10, 20, 30, 40, 50 };
        private MudTable<ApplicationViewModel> applicationsTable;

        private bool resetCurrentPage = false;


        private async Task<TableData<ApplicationViewModel>> GetApplicationsDataAsync(TableState state, CancellationToken ct)
        {
            try
            {

                resetCurrentPage = false;

                var sessionPage = await ApiClient.GetApplicationsEndpointAsync(CancellationToken.None);

                return new TableData<ApplicationViewModel>
                {
                    Items = sessionPage,
                    TotalItems = sessionPage.Count
                };
            }
            catch (Exception ex)
            {
                Toast.Add($"Error while retrieving applications.{ex.Message}", Severity.Error);
            }
            return new TableData<ApplicationViewModel>
            {
                Items = Enumerable.Empty<ApplicationViewModel>(),
                TotalItems = 0
            };
        }

        /// <summary>
        /// Refresh data for the search query
        /// </summary>
        /// <param name="text"></param>
        private void OnSearch(string text)
        {
            // applicationsRequest.ApplicationFilter = string.Empty;
            // if (!string.IsNullOrEmpty(text))
            // {
            //     applicationsRequest.ApplicationFilter = text;
            // }
            resetCurrentPage = true;
            applicationsTable.ReloadServerData();
        }

        /// <summary>
        /// Navigate to add new application page
        /// </summary>
        void AddNewApplication()
        {
            Navigation.NavigateTo($"applications/new");
        }

        /// <summary>
        /// Navigate to edit application page
        /// </summary>
        /// <param name="application"></param>
        void EditApplication(ApplicationViewModel application)
        {
            Navigation.NavigateTo($"applications/edit/{application.ClientId}");
        }

        /// <summary>
        /// Delete the application
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        async Task DeleteApplicationAsync(ApplicationViewModel application)
        {
            bool? dialogResult = await DialogService.ShowMessageBox("Warning", "Delete can't be undone !!",
                yesText: "Delete!", cancelText: "Cancel", options: new DialogOptions() { FullWidth = true });
            if (dialogResult.GetValueOrDefault())
            {
                await ApiClient?.DeleteApplicationEndpointAsync(application.ClientId);
                // if (result.IsSuccess)
                // {
                //     Toast.Add("Deleted successfully.", Severity.Success);
                //     await applicationsTable.ReloadServerData();
                //     return;
                // }
                // Toast.Add(result.ToString(), Severity.Error, config =>
                // {
                //     config.ShowCloseIcon = true;
                //     config.RequireInteraction = true;
                // });
            }
        }      
    }
}
