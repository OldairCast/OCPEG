using OCPEG.Domain.BusinessObject;
 using OCPEG.Domain.DataAccess.Entities;


namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface IAccountRepository 
    {

        Task<User> GetAdm();

        Task<List<Role>> GetRoles(string roleid);

        Task<bool> UpdateToken(User user);

        Task<bool> AddToRoleAsync(int userId, short roleId);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<bool> ResetPasswordAsync(User user, string token, string password);

    }
}
