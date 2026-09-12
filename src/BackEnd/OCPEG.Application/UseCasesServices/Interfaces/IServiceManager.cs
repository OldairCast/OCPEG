namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface IServiceManager
    {
        IAccountService AccountService { get; }

        ICallService CallService { get; }
        
        ICustomerService CustomerService { get; }

        ICustomerTypeService CustomerTypeService { get; }

        IProductService ProductService { get; }

        IResolutionService ResolutionService { get; }

        ITokenService TokenService { get; }

        IUserService UserService { get; }

    }
}
