using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public  interface IUserService : IBaseService<User, UserDto>
    {
        new Task<int> Create(User entity);
    }
}
