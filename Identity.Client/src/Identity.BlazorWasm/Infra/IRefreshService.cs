namespace Identity.BlazorWasm.Infra;

public interface IRefreshService
{
    Task<AuthRefreshResult> RefreshAsync();
}