using OCPEG.Domain.BusinessObject;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface IAccountService
    {
        Task<int> Register(User user);
        Task<TokenOut> Login(User userLogin);
        Task<TokenOut> LoginGoogle(User userLogin);
        Task<TokenOut> RefreshToken(TokenOut tokenModel);
        Task<bool> Revoke(int userId);
        Task<bool> RevokeAll();
        Task<bool> UpdateAccount(User user);
        Task<User> GetUserLogged();
        Task<bool> UserExists(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserMain();

    }
}
