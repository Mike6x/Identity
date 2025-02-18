using Quartz;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Identity.API.Data;
using Identity.API.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Hosting;

public static class OpenIdDictConfig
{
    public static IHostApplicationBuilder ConfigureOpenIddict(this IHostApplicationBuilder app)
    {
        var openIddictSettings = app.Configuration.GetSection("OpenIdDict").Get<OpenIddictSettingsConfig>();

        app.Services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });

        app.Services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<ApplicationDbContext>();

                options.UseQuartz();
            })
            .AddServer(options =>
            {

                //options.EnableStatusCodePagesIntegration()
                //options.AddEventHandler<ProcessErrorContext>(builder =>
                //{
                //    builder.UseInlineHandler(async context =>
                //    {
                //        var errorDescription = context.ErrorDescription;
                //        var errorCode = context.Error;

                //        this doesnt work, other solutions?
                //        context.Response.Redirect($"/error?code={errorCode}&description={errorDescription}", );

                //        context.HandleRequest();
                //    });
                //});


                options
                    .SetAuthorizationEndpointUris("/connect/authorize")
                    .SetTokenEndpointUris("/connect/token")
                    .SetEndSessionEndpointUris("/connect/endsession")
                    .SetIntrospectionEndpointUris("/connect/introspect")
                    .SetUserInfoEndpointUris("/connect/userinfo");
                
                options
                    .SetAccessTokenLifetime(TimeSpan.FromSeconds(3600))
                    .SetRefreshTokenLifetime(TimeSpan.FromSeconds(86400));

                var scopes = new List<string>
                {
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles,
                }.Select(s => s.Replace(Permissions.Prefixes.Scope, ""));

                options.RegisterScopes(scopes.ToArray());

                //allowed grant types
                options.AllowClientCredentialsFlow();
                options.AllowAuthorizationCodeFlow();
                options.AllowRefreshTokenFlow();
                //PKCE
                options.RequireProofKeyForCodeExchange();


                // Register the signing and encryption credentials.
                //todo only dev
                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                if(!string.IsNullOrEmpty(openIddictSettings?.Encryption?.Key))
                {
                    options.AddEncryptionKey(
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(openIddictSettings.Encryption.Key ?? ""))
                            );
                } else if (openIddictSettings?.Encryption?.Cert != null)
                {
                    var path = openIddictSettings.Encryption?.Cert?.Path ?? "./cert.pfx";

                    if (openIddictSettings?.Encryption?.Cert?.GenerateIfEmpty == true)
                        GenerateCerificate(path, openIddictSettings?.Encryption?.Cert, CertificateType.Encryption);

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
                            Convert.FromBase64String(openIddictSettings.Signing.Key ?? ""))
                            );
                }
                else if (openIddictSettings?.Signing?.Cert != null)
                {
                    var path = openIddictSettings.Signing?.Cert?.Path ?? "./cert.pfx";

                    if (openIddictSettings?.Signing?.Cert?.GenerateIfEmpty == true)
                        GenerateCerificate(path, openIddictSettings?.Signing?.Cert, CertificateType.Signing);
                    if (!File.Exists(openIddictSettings?.Signing?.Cert?.Path))
                    {
                        throw new FileNotFoundException($"Certificate not found at {path}");
                    }
                    var cert = X509CertificateLoader.LoadPkcs12FromFile(path, openIddictSettings.Signing.Cert.Password);
                    options.AddSigningCertificate(cert);
                }
              

                var aspBuilder = options.UseAspNetCore()
                       .EnableAuthorizationEndpointPassthrough()
                       .EnableTokenEndpointPassthrough()
                       .EnableEndSessionEndpointPassthrough()
                       .EnableUserInfoEndpointPassthrough();

                if(openIddictSettings?.OnlyAllowHttps != true)
                {
                    aspBuilder.DisableTransportSecurityRequirement();
                }

            });
            //.AddValidation(options =>
            //{
            //    // Note: the validation handler uses OpenID Connect discovery
            //    // to retrieve the address of the introspection endpoint.
            //    //options.SetClientId(openIddictSettings.IdentityClientId);

            //    // Import the configuration from the local OpenIddict server instance.
            //    options.UseLocalServer();

            //    // Register the System.Net.Http. integration
            //    options.UseSystemNetHttp();

            //    // Register the ASP.NET Core host.
            //    options.UseAspNetCore();
            //});



        return app;
    }

    public static IApplicationBuilder UseOpenIddict(this WebApplication app)
    {
        app.UseCors();

        return app;
    }

    private static void GenerateCerificate(string path, CertConfig? cert, CertificateType type)
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
