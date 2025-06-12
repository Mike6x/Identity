using Client.Infrastructure.Api;
using Client.Infrastructure.Auth;
using Identity.Admin.Components.Dialogs;
using Identity.Admin.Components.EntityTable;
using Identity.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Identity.Admin.Components.Pages.Users;

public partial class Users : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IApiClient UsersClient { get; set; } = default!;

    protected EntityClientTableContext<UserDetail, Guid, UserViewModel> Context { get; set; } = default!;

    private bool _canRemoveUsers;
    private bool _canViewAuditTrails;
    private bool _canViewRoles;
    private string _currentUserId = string.Empty;

    // Fields for edit form
    protected string Password { get; set; } = string.Empty;
    protected string ConfirmPassword { get; set; } = string.Empty;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {

        if ((await AuthState).User is { } user)
        {
            _canRemoveUsers = await AuthService.HasPermissionAsync(user, AppActions.Delete, AppResources.Users);
            _canViewRoles = await AuthService.HasPermissionAsync(user, AppActions.View, AppResources.UserRoles);
            _canViewAuditTrails = await AuthService.HasPermissionAsync(user, AppActions.View, AppResources.AuditTrails);
            _currentUserId = user.GetUserId() ?? string.Empty;
        }

        Context = new(
            entityName: "User",
            entityNamePlural: "Users",
            entityResource: AppResources.Users,
            // searchAction: FshActions.View,
            // updateAction: string.Empty,
            // deleteAction: string.Empty,
            // exportAction: string.Empty,
            // importAction: string.Empty,
            fields: new()
            {
                new(user => user.UserName, "UserName"),
                new(user => user.Email, "Email"),
                new(user => user.FirstName,"First Name"),
                new(user => user.LastName, "Last Name"),
                new(user => user.PhoneNumber, "PhoneNumber"),

                new(user => user.IsActive, "Active", Type: typeof(bool)),
                new(user => user.EmailConfirmed, "Email Confirmed", Type: typeof(bool)),
                // new(user => user.LockoutEnd, "LockoutEnd", Type: typeof(DateTime)),
                new(user => user.IsLocked, "Lockout", Type: typeof(bool)),
                new(user => user.IsOnline, "Online", Type: typeof(bool))               
            },
            idFunc: user => user.Id,
            loadDataFunc: async () => (await UsersClient.GetUsersEndpointAsync()).ToList(),
            searchFunc: (searchString, user) =>
                string.IsNullOrWhiteSpace(searchString)
                    || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: user =>
            {
                var createRequest = user.Adapt<CreateUserCommand>();

                if (string.IsNullOrEmpty(createRequest.UserName)) createRequest.UserName = createRequest.Email;

                return UsersClient.CreateUserEndpointAsync(createRequest);
                // return UsersClient.RegisterUserEndpointAsync(createRequest);
            },
            updateFunc: async (id, user) =>
            {
                UpdateUserCommand updateRequest = user.Adapt<UpdateUserCommand>();
                updateRequest.Id = id.ToString();
                updateRequest.LastModifiedBy = _currentUserId;
                updateRequest.LastModifiedOn = DateTime.UtcNow;

                await UsersClient.UpdateUserEndpointAsync(id.ToString(),updateRequest);
            },
            // deleteFunc: async id => await UsersClient.DisableUserEndpointAsync(id.ToString()),
            deleteFunc: async id =>
            {
                var request = new ToggleUserStatusCommand { UserId = id.ToString(), IsActive = true };
                await UsersClient.ToggleUserStatusEndpointAsync(id.ToString(), request);
            },
            exportFunc: async filter =>
            {
                var dataFilter = filter.Adapt<ExportUsersRequest>();

                return await UsersClient.ExportUsersEndpointAsync(dataFilter);
            },
            importFunc: async (fileUploadModel, isUpdate) => await UsersClient.ImportUsersEndpointAsync(isUpdate, fileUploadModel),
            hasExtraActionsFunc: () => true);
    }

    private void ViewProfile(in Guid userId) =>
        Navigation.NavigateTo($"/identity/users/{userId}/profile");

    private void ManageRoles(in Guid userId) =>
        Navigation.NavigateTo($"/identity/users/{userId}/roles");
    private void ViewAuditTrails(in Guid userId) =>
        Navigation.NavigateTo($"/identity/users/{userId}/audit-trail");

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }

        Context.AddEditModal.ForceRender();
    }

    private async Task RemoveUserAsync(Guid userId) 
    {
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, BackdropClick = false };
        const string contentText = "You're sure you want to remove this user ?";
        
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), contentText }
        };
        var dialog = await DialogService.ShowAsync<DeleteConfirmation>("Remove", parameters, options);
        
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            _ = UsersClient.DeleteUserEndpointAsync(userId.ToString());
             await OnInitializedAsync();
            //_ = Context.LoadDataFunc()
        }

    }

}

public class UserViewModel : UpdateUserCommand
{

    // private bool _isLocked => LockoutEnd != null && LockoutEnd > DateTime.UtcNow

}
