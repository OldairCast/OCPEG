using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Application.UseCasesServices.Services;

namespace OCPEG.Application.Extension
{
    public static class AplicationExtension
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            AddServices(services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerImageService, CustomerImageService>();
            services.AddScoped<ICustomerTypeService, CustomerTypeService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICallService, CallService>();
            services.AddScoped<IResolutionService, ResolutionService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICloudImageService, CloudImageService>();
            services.AddScoped<ICloudMessageService, CloudMessageService>();
        }
    }
}
