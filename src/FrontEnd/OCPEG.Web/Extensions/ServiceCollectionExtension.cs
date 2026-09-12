using Microsoft.AspNetCore.Authentication.Cookies;
using OCPEG.Web.Hosts;
using OCPEG.Web.Hosts.IHosts;
using OCPEG.Web.Services;
using OCPEG.Web.Services.IServices;

namespace OCPEG.Web.Extensions
{
    /// <summary>
    /// Extensão do Program.cs
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Configuração para padrão MicroServiços
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var baseUrl = configuration["ServiceUrls:AccessControlAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:AccessControlAPI não configurado.");

            services.AddHttpClient<IAccessControlHost, AccessControlHost>(
                c => c.BaseAddress = new Uri(baseUrl)
            );

            baseUrl = configuration["ServiceUrls:CallAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:CallAPI não configurado.");

            services.AddHttpClient<ICallHost, CallHost>(c =>
                    c.BaseAddress = new Uri(baseUrl)
                );

            baseUrl = configuration["ServiceUrls:CustomerAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:CustomerAPI não configurado.");

            services.AddHttpClient<ICustomerHost, CustomerHost>(c =>
                    c.BaseAddress = new Uri(baseUrl)
                );



            baseUrl = configuration["ServiceUrls:OpenAIAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:OpenAIAPI não configurado.");

            services.AddHttpClient<IOpenAIHost, OpenAIHost>(c =>
                    c.BaseAddress = new Uri(baseUrl)
                );
        }

        /// <summary>
        /// Registra Dependencias
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddGoogleAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var clientId = configuration.GetValue<string>("Google:ClientId")!;
            var clientSecret = configuration.GetValue<string>("Google:ClientSecret")!;

            services.AddAuthentication(config =>
            {
                config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie()
            .AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = clientId;
                googleOptions.ClientSecret = clientSecret;
            });
        }
    }
}
