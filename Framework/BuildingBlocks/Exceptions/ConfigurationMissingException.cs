namespace BuildingBlocks.Exceptions;

public class ConfigurationMissingException : NotFoundException
{
    public ConfigurationMissingException(string sectionName) : base($"{sectionName} Missing in Configurations")
    {
    }
    
}