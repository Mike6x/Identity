namespace Identity.Core.Features.Scope.Create;

public class CreateScopeCommand
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    
    public HashSet<string>? Resources { get; set; } = [];
    
}