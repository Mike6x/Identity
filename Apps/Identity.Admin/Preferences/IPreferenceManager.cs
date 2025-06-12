namespace Identity.Admin.Preferences;

public interface IPreferenceManager
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<bool> ChangeLanguageAsync(string languageCode);
}