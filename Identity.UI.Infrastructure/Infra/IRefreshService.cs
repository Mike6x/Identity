namespace Identity.UI.Infrastructure.Infra;

public interface IRefreshService
{
    Task<AuthRefreshResult> RefreshAsync();
}