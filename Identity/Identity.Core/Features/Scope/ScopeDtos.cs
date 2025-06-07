namespace Identity.Core.Features.Scope;

public record ScopeDto(
    string Name,
    string DisplayName,
    string Description,
    HashSet<string>? Resources = null
);

public record ScopeSummaryDto(
    string Id,
    string Name,
    string DisplayName,
    string Description
);

public record EditScopeDto(
    string Id,
    string Name,
    string DisplayName,
    string Description,
    HashSet<string>? Resources = null
);

// public class ScopeDto
// {
//     public string Id { get; set; } = string.Empty;
//
//     public string Name { get; set; } = string.Empty;
//
//     public string DisplayName { get; set; } = string.Empty;
//
//     public string Description { get; set; } = string.Empty;
//
//     public List<string> Resources { get; set; } = [];
// }

// public class ScopeInfo
// {
//     public string Id { get; set; } = string.Empty;
//
//     public string Name { get; set; } = string.Empty;
//
//     public string DisplayName { get; set; } = string.Empty;
//
//     public string Description { get; set; } = string.Empty;
//     
//     public List<string> Resources { get; set; } = [];
// }