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
    protected Task<AuthenticationState>? AuthState { get; set; }
    [Inject]
    protected IAuthorizationService? AuthService { get; set; }
    [Inject]
    public NavigationManager? Navigator { get; set; }

    [Inject]
    protected IApiClient? ApiClient { get; set; }

    public required IDialogService Dialog;
    private EntityClientTableContext<UserSummaryDto, Guid, UserViewModel> Context { get; set; }
    

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
        if (ApiClient == null) return;
        if (AuthState == null)  return;
        
        var state = await AuthState;
        if (AuthService != null)
        {
            _canRemoveUsers = await AuthService.HasPermissionAsync(state.User, AppActions.Delete, AppResources.Users);
            _canViewRoles = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.UserRoles);
            _canViewAuditTrails = await AuthService.HasPermissionAsync(state.User, AppActions.View, AppResources.AuditTrails);
            
            _currentUserId = state.User.GetUserId() ?? string.Empty;
        }
 
        // if ((await AuthState).User is { } user)
        // {
        //     _canRemoveUsers = await AuthService.HasPermissionAsync(user, AppActions.Delete, AppResources.Users);
        //     _canViewRoles = await AuthService.HasPermissionAsync(user, AppActions.View, AppResources.UserRoles);
        //     _canViewAuditTrails = await AuthService.HasPermissionAsync(user, AppActions.View, AppResources.AuditTrails);
        //     _currentUserId = user.GetUserId() ?? string.Empty;
        // }
        
        Context = new EntityClientTableContext<UserSummaryDto, Guid, UserViewModel>(
            entityName: "User",
            entityNamePlural: "Users",
            entityResource: AppResources.Users,
            // searchAction: FshActions.View,
            // updateAction: string.Empty,
            // deleteAction: string.Empty,
            // exportAction: string.Empty,
            // importAction: string.Empty,
            fields:
            [
                new EntityField<UserSummaryDto>(user => user.UserName, "UserName"),
                new EntityField<UserSummaryDto>(user => user.Email, "Email"),
                new EntityField<UserSummaryDto>(user => user.FirstName, "First Name"),
                new EntityField<UserSummaryDto>(user => user.LastName, "Last Name"),
                new EntityField<UserSummaryDto>(user => user.PhoneNumber, "PhoneNumber"),

                new EntityField<UserSummaryDto>(user => user.IsActive, "Active", Type: typeof(bool)),
                new EntityField<UserSummaryDto>(user => user.EmailConfirmed, "Email Confirmed", Type: typeof(bool)),
                //new EntityField<UserSummaryDto>(user => user.LockoutEnd, "LockoutEnd", Type: typeof(DateTime)),
                new EntityField<UserSummaryDto>(user => user.IsLocked, "Locked", Type: typeof(bool)),
                new EntityField<UserSummaryDto>(user => user.IsOnline, "Online", Type: typeof(bool))
            ],
            idFunc: user => user.Id,
            loadDataFunc: async () => (await ApiClient.GetUsersEndpointAsync()).ToList(),
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

                return ApiClient.CreateUserEndpointAsync(createRequest);
                // return ApiClient.RegisterUserEndpointAsync(createRequest);
            },
            updateFunc: async (id, user) =>
            {
                var updateRequest = user.Adapt<UpdateUserCommand>();
                updateRequest.Id = id.ToString();
                updateRequest.LastModifiedBy = _currentUserId;
                updateRequest.LastModifiedOn = DateTime.UtcNow;

                await ApiClient.UpdateUserEndpointAsync(id.ToString(),updateRequest);
            },
            // deleteFunc: async id => await ApiClient.DisableUserEndpointAsync(id.ToString()),
            deleteFunc: async id =>
            {
                var request = new ToggleUserStatusCommand { UserId = id.ToString(), IsActive = true };
                await ApiClient.ToggleUserStatusEndpointAsync(id.ToString(), request);
            },
            exportFunc: async filter =>
            {
                var dataFilter = filter.Adapt<ExportUsersRequest>();

                return await ApiClient.ExportUsersEndpointAsync(dataFilter);
            },
            importFunc: async (fileUploadModel, isUpdate) => await ApiClient.ImportUsersEndpointAsync(isUpdate, fileUploadModel),
            hasExtraActionsFunc: () => true);
    }

    private void ToUserDetails(in Guid userId) =>
        Navigator?.NavigateTo($"/identity/users/{userId}/details");
    
    // private void ToEditUser(in Guid userId) =>
    //     Navigator?.NavigateTo($"/identity/users/{userId}/edit");

    private void ToUserRoles(in Guid userId) =>
        Navigator?.NavigateTo($"/identity/users/{userId}/roles");
    
    private void ToUserClaims(in Guid userId) =>
        Navigator?.NavigateTo($"/identity/users/{userId}/claims");
    private void ViewAuditTrails(in Guid userId) =>
        Navigator?.NavigateTo($"/identity/users/{userId}/audit-trail");

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
        var dialog = await Dialog.ShowAsync<DeleteConfirmation>("Remove", parameters, options);
        
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            _ = ApiClient?.DeleteUserEndpointAsync(userId.ToString());
             await OnInitializedAsync();
            //_ = Context.LoadDataFunc()
        }

    }

}

public class UserViewModel : UpdateUserCommand
{

    // private bool _isLocked => LockoutEnd != null && LockoutEnd > DateTime.UtcNow

}
