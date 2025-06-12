using Identity.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Identity.Admin.Components.General;

public partial class PersonCard
{
    [Parameter]
    public string? Class { get; set; }
    [Parameter]
    public string? Style { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = null!;

    private string? UserId { get; set; }
    private string? Email { get; set; }
    private string? FullName { get; set; }
    private string? ImageUri { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadUserData();
        }
    }

    private async Task LoadUserData()
    {
        var user = (await AuthState).User;
        if (user.Identity?.IsAuthenticated == true && string.IsNullOrEmpty(UserId))
        {
            FullName = user.GetFullName();
            UserId = user.GetUserId();
            Email = user.GetEmail();
            if (user.GetImageUrl() != null)
            {
                ImageUri = user.GetImageUrl()?.ToString();
            }
            StateHasChanged();
        }
    }
}
