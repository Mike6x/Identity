namespace Identity.Core.Features.Scope.Update;

public class UpdateScopeCommand
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public HashSet<string> Resources { get; set; } = [];
    
}