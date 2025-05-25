namespace Identity.Shared.Auth;

/// <summary>
/// Constants for authorization policies.
/// </summary>
public static class PolicyConstants
{
    /// <summary>
    /// Policy for test resource server 
    /// </summary>
        
    public const string AuthPolicy = "AuthPolicy";
    
    public const string ReadWeatherDataPolicy = nameof(ReadWeatherDataPolicy);
    
    /// <summary>
    /// Policy to get user data.
    /// </summary>
    public const string GetEmployee = "get-employee";

    /// <summary>
    /// Policy to get weather conditions.
    /// </summary>
    public const string GetWeather = "get-weather";

}