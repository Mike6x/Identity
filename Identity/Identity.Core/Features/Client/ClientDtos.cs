using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Core.Features.Client;

public record ClientDto(
    string ClientId,
    string ClientSecret,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUris,
    HashSet<string> AllowedScopes
);

public record ClientSummaryDto(
    string Id,
    string ClientId,
    string DisplayName
);

public record EditClientDto(
    string Id,
    string ClientId,
    string DisplayName,
    string RedirectUri,
    string PostLogoutRedirectUris,
    HashSet<string> AllowedScopes
);

public class ApplicationSummaryDto
{
    public string Id { get; set; } = string.Empty;

    public string ApplicationType { get; set; } = ApplicationTypes.Web;

    public string ClientId { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public bool IsConfidentialClient => ClientType?.Equals(ClientTypes.Confidential) ?? false;

    public string? ClientSecret { get; set; }

    public string? JsonWebKeySet { get; set; }

    public string ConsentType { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}


public class ApplicationDto
{
    public string Id { get; set; } = string.Empty;

    public string ApplicationType { get; set; } = ApplicationTypes.Web;

    public string ClientId { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
    public bool IsConfidentialClient => ClientType?.Equals(ClientTypes.Confidential) ?? false;

    public string? ClientSecret { get; set; }

    public string? JsonWebKeySet { get; set; }

    public string ConsentType { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public List<string>? Permissions { get; set; } = [];

    public List<Uri>? RedirectUris { get; set; } = [];

    public List<Uri>? PostLogoutRedirectUris { get; set; } = [];

    public List<string>? Requirements { get; set; } = [];

    public Dictionary<string, string>? Settings { get; set; } = new(StringComparer.Ordinal);
}