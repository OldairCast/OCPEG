using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Base;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task<bool> ExistActiveUserWithEmail(string email);
        Task<User> GetByEmail(string email);

        Task<User> GetByIdentifier(Guid userIdentifier);
    }
}
