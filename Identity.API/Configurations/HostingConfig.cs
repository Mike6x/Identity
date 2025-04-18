using Microsoft.AspNetCore.HttpOverrides;

namespace Identity.API.Configurations;

public static class HostingConfig
{
    public static IApplicationBuilder UseUrlsFromConfig(this WebApplication app)
    {
        var httpsRedirection = app.Configuration.GetSection("Configurations:HttpsRedirection")?.Get<bool>() != false;
        if (httpsRedirection)
            app.UseHttpsRedirection();

        var urls = app.Configuration.GetSection("Configurations:Urls")?.Get<string[]>();
        if (urls != null && urls.Any())
        {
            app.Urls.Clear();
            foreach (var url in urls)
            {
                app.Urls.Add(url);
            }
        }
        return app;
    }
    
    public static IHostApplicationBuilder ConfigureReverseProxySupport(this IHostApplicationBuilder app)
    {

        var rProxySupport = app.Configuration.GetSection("Configurations:ReverseProxySupport")?.Get<bool>() != false;
        if(rProxySupport)
            app.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
            });

        return app;
    }

    public static WebApplication UseReverseProxySupport(this WebApplication app)
    {
        var rProxySupport = app.Configuration.GetSection("Configurations:ReverseProxySupport")?.Get<bool>() != false;

        if(rProxySupport)
        {
            app.UseCertificateForwarding();
            app.UseForwardedHeaders();
        }

        var assumeEveryRequestHttps = app.Configuration.GetSection("Configurations:AssumeEveryRequestHttps")?.Get<bool>() == true;
        if (assumeEveryRequestHttps)
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";

                return next();
            });

        return app;
    }

}