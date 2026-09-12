using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;

namespace OCPEG.Web.Services.IServices
{
    public interface IAccessControlHost
    {

        #region Login
        Task<ResponseResult<TokenOut>> Login(UserLoginDto userLogin);

        Task<ResponseResult<TokenOut>> LoginGoogle(UserDto user);

        Task<ResponseResult<bool>> Register(UserDto userRegister);

        #endregion


        #region Usuário
        Task<ResponseResult<List<UserDto>>> GetUserAll(string token);

        Task<ResponseResult<int>> CreateUser(string token, UserDto user);

        Task<ResponseResult<UserDto>> GetUserById(string token, string id);

        Task<ResponseResult<UserDto>> GetUserByName(string token, string name);

        Task<ResponseResult<bool>> UpdateUser(string token, UserDto user);

        Task<ResponseResult<bool>> DeleteUserById(string token, string id);

        Task<ResponseResult<UserDto>> GetUserIdentifierLogged(string token, string userIdentifier);

        #endregion
    }
}
