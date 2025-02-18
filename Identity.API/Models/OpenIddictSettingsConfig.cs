namespace Identity.API.Models;

public class OpenIddictSettingsConfig
{
    public bool OnlyAllowHttps { get; set; }
    public EncryptionConfig Encryption { get; set; }
    public EncryptionConfig Signing { get; set; }

    public IEnumerable<ApplicationConfig> ApplicationConfigs { get; set; }
}

public class EncryptionConfig
{
    public string Key { get; set; }
    public CertConfig Cert { get; set; }
}

public class CertConfig
{
    public string Path { get; set; }
    public bool GenerateIfEmpty { get; set; }
    public string Password { get; set; }
    public int ValidityMonths { get; set; }
    public string Issuer { get; set; } = "OpenIddict";
}
