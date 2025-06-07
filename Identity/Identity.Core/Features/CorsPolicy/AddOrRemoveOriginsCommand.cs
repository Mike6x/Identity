namespace Identity.Core.Features.CorsPolicy;

public class AddOrRemoveOriginsCommand
{
    public List<string> Origins { get; set; } = [];
}