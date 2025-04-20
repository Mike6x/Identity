using Microsoft.AspNetCore.Components;

namespace Identity.BlazorWasm.Infra;

public interface ILoginService
{
    Task<LoginResult?> LoginUserAsync(string userName, string password);
}