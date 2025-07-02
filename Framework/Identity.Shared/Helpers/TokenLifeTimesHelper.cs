using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Shared.Helpers;

public static class TokenLifeTimesHelper
{
    public static IEnumerable<string> TokenLifeTimeNames { get; } = new List<string>()
    {
        nameof(Settings.TokenLifetimes.AccessToken),
        nameof(Settings.TokenLifetimes.AuthorizationCode),
        nameof(Settings.TokenLifetimes.DeviceCode),
        nameof(Settings.TokenLifetimes.IdentityToken),
        nameof(Settings.TokenLifetimes.RefreshToken),
        nameof(Settings.TokenLifetimes.UserCode)
    };

    public static string GetValueFromName(string  settingName)
    {
        return settingName switch
        {
            nameof(Settings.TokenLifetimes.AccessToken) => Settings.TokenLifetimes.AccessToken,
            nameof(Settings.TokenLifetimes.AuthorizationCode) => Settings.TokenLifetimes.AuthorizationCode,
            nameof(Settings.TokenLifetimes.DeviceCode) => Settings.TokenLifetimes.DeviceCode,
            nameof(Settings.TokenLifetimes.IdentityToken) => Settings.TokenLifetimes.IdentityToken,
            nameof(Settings.TokenLifetimes.RefreshToken) => Settings.TokenLifetimes.RefreshToken,
            nameof(Settings.TokenLifetimes.UserCode) => Settings.TokenLifetimes.UserCode,
            _ => throw new ArgumentException($"{settingName} doesn't exist", nameof(settingName))
        };
    }

    public static string GetNameFromValue(string settingValue)
    {
        return settingValue switch
        {
            Settings.TokenLifetimes.AccessToken => nameof(Settings.TokenLifetimes.AccessToken),
            Settings.TokenLifetimes.AuthorizationCode => nameof(Settings.TokenLifetimes.AuthorizationCode),
            Settings.TokenLifetimes.DeviceCode => nameof(Settings.TokenLifetimes.DeviceCode),
            Settings.TokenLifetimes.IdentityToken => nameof(Settings.TokenLifetimes.IdentityToken),
            Settings.TokenLifetimes.RefreshToken => nameof(Settings.TokenLifetimes.RefreshToken),
            Settings.TokenLifetimes.UserCode => nameof(Settings.TokenLifetimes.UserCode),
            _ => throw new ArgumentException($"{settingValue} doesn't exist", nameof(settingValue))
        };
    }
}
