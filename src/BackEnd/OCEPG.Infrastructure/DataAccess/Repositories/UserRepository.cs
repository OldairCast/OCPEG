using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;

namespace OCEPG.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext, IMapper mapper) : base(mapper)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistActiveUserWithEmail(string email) => await _dbContext.Usuario.AnyAsync(user => email.Equals(user.Email) && user.Active);

        /// <summary>
        /// Retorna lista de usuários
        /// </summary>
        /// <returns>Lista de Objeto de negocio User</returns>
        public async Task<List<User>> GetAll()
        {
            try
            {
                List<User> userlst = new List<User>();

                List<Usuario>? usuariolst = await _dbContext.Usuario.AsNoTracking().ToListAsync();

                if (usuariolst != null && usuariolst.Count > 0)
                {
                    foreach (var item in usuariolst)
                    {
                        User user = _mapper.Map<User>(item);
                        userlst.Add(user);
                    }
                }

                return userlst;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Obtém dados do usuário pelo id do usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns>Objeto de negocio User</returns>
        public async Task<User> GetById(int id)
        {
            try
            {
                User user = new User();
                Usuario? usuario = await _dbContext.Usuario
                                .Where(b => b.Id == id)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    user = _mapper.Map<User>(usuario);
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
        /// Obtém dados do usuário pelo email
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <returns>Objeto de negocio User</returns>
        public async Task<User> GetByEmail(string email)
        {
            try
            {
                User user = new User();
                Usuario? usuario = await _dbContext.Usuario
                                .Where(b => b.Email == email)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    user = _mapper.Map<User>(usuario);
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
        /// Obtém dados do usuário pelo identificador do usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns>Objeto de negocio User</returns>
        public async Task<User> GetByIdentifier(Guid userIdentifier)
        {
            try
            {
                User user = new User();
                Usuario? usuario = await _dbContext.Usuario
                                .Where(b => b.UserIdentifier == userIdentifier)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();

                if (usuario != null)
                {
                    user = _mapper.Map<User>(usuario);
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
        /// Insere um novo usuário
        /// </summary>
        /// <returns>Id do usuário criado</returns>
        public async Task<int> Create(User user)
        {
            try
            {
                Usuario usuario = new Usuario
                {
                    Id = user.Id,
                    Nome = user.Name,
                    Email = user.Email,
                    Telefone = user.PhoneNumber,
                    PasswordHash = user.PasswordHash,
                    Active = user.Active,
                    Administrador = user.Admin,
                    Funcao = user.Function,
                    RefreshToken = user.RefreshToken,
                    RefreshTokenExpiryTime = DateTime.Now
                };

                await _dbContext.Usuario.AddAsync(usuario);
                await _dbContext.SaveChangesAsync();

                User userRet = _mapper.Map<User>(usuario);
                return userRet.Id;

            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Atualiza um usuário
        /// </summary>
        /// <param name="user">Dados do usuário</param> 
        public async Task<bool> Update(User user)
        {
            try
            {
                Usuario? usuario = await _dbContext.Usuario.FindAsync(user.Id);
                if (usuario != null)
                {
                    usuario.Nome = user.Name;
                    usuario.Email = user.Email;
                    usuario.Telefone = user.PhoneNumber;
                    usuario.Administrador = user.Admin;  
                    usuario.Funcao = user.Function;

                    _dbContext.Usuario.Update(usuario);
                }

                ////Quem executa a transação no banco é o UnitofWork
                //await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }

        /// <summary>
        /// Exclui um usuário
        /// </summary>
        /// <param name="id">Id do usuário</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                Usuario? user = await _dbContext.Usuario.FindAsync(id);

                if (user != null)
                {
                    _dbContext.Usuario.Remove(user);
                    
                    //Quem executa a transação no banco é o UnitofWork
                    //await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw;
            }
        }
    }
}
