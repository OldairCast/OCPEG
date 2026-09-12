using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private readonly AppDbContext _dbContext;

        public AccountRepository(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }


        /// <summary>
        /// Obtém dados do usuário Adm
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Objeto de negocio User</returns>
        public async Task<User> GetAdm()
        {
            try
            {
                User user = new User();

                Usuario? usuario = await _dbContext.Usuario.FindAsync(user.Admin);
                if (usuario != null)
                {
                    user = _mapper.Map<User>(usuario);

                    user.Id = usuario.Id;
                    user.Name = usuario.Nome;
                    user.Email = usuario.Email;
                    user.PhoneNumber = usuario.Telefone;
                    user.Active = usuario.Active;
                }

                return user;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Obtém Role por id
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Objeto de negocio Role</returns>
        public async Task<List<Role>> GetRoles(string roleid)
        {
            try
            {
                var roles = await _dbContext.Role
                                .Where(b => b.RoleId == roleid)
                                .AsNoTracking()
                                .ToListAsync();

                return roles;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza o token do usuário
        /// </summary>
        /// <param name="user">Dados do usuário</param> 
        public async Task<bool> UpdateToken(User user)
        {
            try
            {
                Usuario? usuario = await _dbContext.Usuario.FindAsync(user.Id);
                if (usuario != null)
                {
                    usuario.RefreshToken = user.RefreshToken;
                    usuario.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;

                    _dbContext.Usuario.Update(usuario);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        public async Task<bool> AddToRoleAsync(int userId, short roleId)
        {
            try
            {
                UsuarioRole userRole = new UsuarioRole
                {
                    UserId = userId,
                    RoleId = roleId
                };

                await _dbContext.UsuarioRole.AddAsync(userRole);
                await _dbContext.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        public Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(User user, string token, string password)
        {
            throw new NotImplementedException();
        }

    }
}
