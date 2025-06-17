using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Identity.Core.Settings;
using Identity.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Identity.Provider.Configurations;

public static class OpenIdDictConfig
{
    public static IServiceCollection AddOpenIdDictConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var openIddictSettings = configuration.GetSection("OpenIddict").Get<OpenIddictSettingsConfig>();

        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();

                options.UseQuartz();
            })
            .AddServer(options =>
            {
                options
                    .SetAuthorizationEndpointUris("/connect/authorize")
                    .SetIntrospectionEndpointUris("/connect/introspect")
                    .SetEndSessionEndpointUris("/connect/logout")
                    .SetTokenEndpointUris("/connect/token")
                    .SetUserInfoEndpointUris("/connect/userinfo")
                    // .SetDeviceAuthorizationEndpointUris("connect/device")
                    .SetEndUserVerificationEndpointUris("connect/verify");
                    
                    // .SetDeviceAuthorizationEndpointUris("/connect/deviceauthorization")
                    // .SetRevocationEndpointUris("/connect/revoke")
                    // .SetJsonWebKeySetEndpointUris("/well-known/jwks.json")
                    // .SetEndUserVerificationEndpointUris("/connect/enduserverification");
                
                //allowed grant types
                options.AllowAuthorizationCodeFlow();
                options.AllowHybridFlow();
                options.AllowClientCredentialsFlow(); // For Machine-to-Machine Authentication
                options.AllowRefreshTokenFlow();
                options.AllowPasswordFlow();
                // options.AllowDeviceAuthorizationFlow();
                
                //options.AllowPasswordFlow().AllowRefreshTokenFlow();
                // options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
                // options.AllowClientCredentialsFlow().AllowRefreshTokenFlow();
                
                options.RequireProofKeyForCodeExchange();
                
                options.RegisterScopes(
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.Roles,
                    Scopes.OfflineAccess,
                    "api"
                );

                // RegisterUser the signing and encryption credentials.
                // todo only dev
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();
                
                # region Check PKCE key
                if(!string.IsNullOrEmpty(openIddictSettings?.Encryption.Key))
                {
                    options.AddEncryptionKey(
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(openIddictSettings.Encryption.Key)));
                    
                } else if (openIddictSettings?.Encryption.Cert != null)
                {
                    var path = openIddictSettings.Encryption?.Cert?.Path ?? "./cert.pfx";

                    if (openIddictSettings?.Encryption?.Cert?.GenerateIfEmpty == true)
                        GenerateCertificate(path, openIddictSettings?.Encryption?.Cert, CertificateType.Encryption);

                    if (!File.Exists(openIddictSettings?.Encryption?.Cert?.Path))
                    {
                        throw new FileNotFoundException($"Certificate not found at {path}");
                    }

                    var cert = X509CertificateLoader.LoadPkcs12FromFile(path, openIddictSettings.Signing.Cert.Password);
                    options.AddEncryptionCertificate(cert);
                }

                if (!string.IsNullOrEmpty(openIddictSettings?.Signing?.Key))
                {
                    options.AddSigningKey(
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(openIddictSettings.Signing.Key ?? "")));
                }
                else if (openIddictSettings?.Signing?.Cert != null)
                {
                    var path = openIddictSettings.Signing?.Cert?.Path ?? "./cert.pfx";

                    if (openIddictSettings?.Signing?.Cert?.GenerateIfEmpty == true)
                        GenerateCertificate(path, openIddictSettings?.Signing?.Cert, CertificateType.Signing);
                    if (!File.Exists(openIddictSettings?.Signing?.Cert?.Path))
                    {
                        throw new FileNotFoundException($"Certificate not found at {path}");
                    }
                    var cert = X509CertificateLoader.LoadPkcs12FromFile(path, openIddictSettings.Signing.Cert.Password);
                    options.AddSigningCertificate(cert);
                }
                
                #endregion
                
                var aspBuilder = options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .EnableStatusCodePagesIntegration();

                if(openIddictSettings?.OnlyAllowHttps != true)
                {
                    aspBuilder.DisableTransportSecurityRequirement();
                }
                
                options.SetAccessTokenLifetime(TimeSpan.FromHours(1))
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(7));

            })
            
            // Check this session
            
            .AddClient(options =>
            {
                options.AllowAuthorizationCodeFlow().AllowRefreshTokenFlow();
            
                options.AddDevelopmentEncryptionCertificate().AddDevelopmentSigningCertificate();
            
                options.UseAspNetCore().EnableRedirectionEndpointPassthrough();
            
                options.UseSystemNetHttp();
            
                // Register the Google integration.
                options.UseWebProviders().AddGoogle(opt =>
                {
                    opt.SetClientId("client_id")
                        .SetClientSecret("client_secrets")
                        .SetRedirectUri("/signin-google")
                        .SetProviderDisplayName("Sign In With Google")
                        .AddScopes("email profile");
                });
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
                options.UseSystemNetHttp();
            });
   
        return services;
    }

    public static IApplicationBuilder UseOpenIddict(this WebApplication app)
    {
        app.UseCors();

        return app;
    }

    private static void GenerateCertificate(string path, CertConfig? cert, CertificateType type)
    {
        if(File.Exists(path))
        {
            //check validity

            var certLoaded = X509CertificateLoader.LoadPkcs12FromFile(path, cert?.Password);
            if (certLoaded.NotAfter.AddDays(-5) > DateTimeOffset.UtcNow)
            {
                var days = (certLoaded.NotAfter - DateTimeOffset.UtcNow).Days;
                Console.WriteLine($"Certificate at {path} is still valid for {days} days");
                return;
            }
            Console.WriteLine($"Certificate at {path} is expired, generating new one");
            if(File.Exists(path+ ".bak"))
            {
                File.Delete(path + ".bak");
            }
            File.Move(path, path + ".bak");
        }

        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName($"CN={cert?.Issuer ?? "OpenIddictSelfSigned"}");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        switch(type)
        {
            case CertificateType.Encryption:
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));
                break;
            case CertificateType.Signing:
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));
                break;
        }



        var validityInMonths = cert?.ValidityMonths ?? 1;
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMonths(validityInMonths).AddDays(1));

        File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, cert?.Password));
    }

    private enum CertificateType
    {
        Encryption,
        Signing
    }
}
