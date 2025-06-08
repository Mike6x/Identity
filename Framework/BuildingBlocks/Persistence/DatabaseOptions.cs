using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Persistence;
public class DatabaseOptions : IValidatableObject
{
    public string Provider { get; set; } =  DbProviders.PostgreSql; // "PostgreSql"
    public string ConnectionString { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            yield return new ValidationResult("connection string cannot be empty.", [nameof(ConnectionString)]);
        }
    }
}
