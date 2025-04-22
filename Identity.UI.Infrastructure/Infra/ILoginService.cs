namespace Identity.UI.Infrastructure.Infra;

public interface ILoginService
{
    Task<LoginResult?> LoginUserAsync(string userName, string password);
}