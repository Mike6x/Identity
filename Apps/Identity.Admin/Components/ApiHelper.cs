using Client.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Identity.Admin.Components;

public static class ApiHelper
{
    public static async Task<T?> ExecuteCallGuardedAsync<T>(
        Func<Task<T>> call,
        ISnackbar snackbar,
        NavigationManager navigationManager,
        AppValidation? customValidation = null,
        string? successMessage = null)
    {
        customValidation?.ClearErrors();
        try
        {
            var result = await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Info);
            }

            return result;
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == 401)
            {
                navigationManager.NavigateTo("/logout");
            }
            var message = ex.Message switch
            {
                "TypeError: Failed to fetch" => "Unable to Reach API",
                _ => ex.Message
            };
            snackbar.Add(message, Severity.Error);
        }

        return default;
    }

    public static async Task<bool> ExecuteCallGuardedAsync(
        Func<Task> call,
        ISnackbar snackbar,
        AppValidation? customValidation = null,
        string? successMessage = null)
    {
        customValidation?.ClearErrors();
        try
        {
            await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Success);
            }

            return true;
        }
        catch (ApiException ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
        }

        return false;
    }
}
