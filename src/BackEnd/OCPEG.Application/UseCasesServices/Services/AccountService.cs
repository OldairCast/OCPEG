using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using System.Security.Cryptography;
using System.Text;
using static OCPEG.Domain.Enum.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ILoggedUser _loggedUser;
        public AccountService(IAccountRepository accountRepository,
                              IUserRepository userRepository,
                              ITokenService tokenService,
                              ILoggedUser loggedUser)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _loggedUser = loggedUser;
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="userDto">Objeto de Negocio de Usuário</param>
        /// <returns></returns>
        public async Task<int> Register(User user)
        {
            try
            {
                user.Email = string.IsNullOrEmpty(user.Email) ? "" : user.Email.ToLower().Trim();
                short userRole = 0;

                if (await UserExists(user.Email))
                    throw new BusinessLogicCustomException(string.Format("{0}", OcPegResource.EMAIL_ALREADY));

                if (user.Password != user.RetypePassword)
                {
                    throw new BusinessLogicCustomException(string.Format("{0}", OcPegResource.RETYPE_PASSWORD));
                }

                if (user.Admin)
                {
                    userRole = Convert.ToByte(UserRolesEn.Admin);
                    user.Function = Convert.ToByte(FunctionEn.Analista);
                }
                else
                {
                    userRole = Convert.ToByte(UserRolesEn.User);
                    user.Function = Convert.ToByte(FunctionEn.Participante);
                }
                    
                //Gera senha hash
                // Generate a 128-bit salt using a sequence of
                // cryptographically strong random bytes.
                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Constants.PasswordDefault,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                user.PasswordHash = hashed;

                //Insere o usuário
                var userId = await _userRepository.Create(user);

                if (userId == 0)
                {
                    throw new BusinessLogicCustomException("Erro ao criar usuário");
                }

                //Insere a Role - Perfil do usuário
                bool resultX = await _accountRepository.AddToRoleAsync(userId, userRole);

                if (!resultX)
                {
                    throw new BusinessLogicCustomException("Erro ao criar usuário");
                }

                return userId;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Login do Usuario
        /// </summary>
        /// <param name="userLogin">Objeto de Negocio de Login</param>
        /// <returns>Informações de Login</returns>
        public async Task<TokenOut> Login(User userLogin)
        {
            try
            {
                string email = string.IsNullOrEmpty(userLogin.Email) ? "" : userLogin.Email.ToLower().Trim();
                string password = string.IsNullOrEmpty(userLogin.Password) ? "" : userLogin.Password.Trim();    

                var objUser = await _userRepository.GetByEmail(email) ?? throw new BusinessLogicCustomException(string.Format("{0}", OcPegResource.INVALID_USERNAME_PASSWORD));

                //Gera senha Hash
                string loginPasswordHash = GeneratePasswordHash(password);

                bool result = CheckUserPasswordAsync(objUser.PasswordHash!, loginPasswordHash);
                if (!result)
                {
                    throw new BusinessLogicCustomException(string.Format("{0}", OcPegResource.INVALID_USERNAME_PASSWORD));
                }

                Task<TokenOut> objToken = _tokenService.CreateToken(objUser);

                return objToken.Result;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Login do Usuario
        /// </summary>
        /// <param name="userLogin">Objeto de Negocio de Login</param>
        /// <returns>Informações de Login</returns>
        public async Task<TokenOut> LoginGoogle(User userLogin)
        {
            try
            {
                Task<TokenOut> objToken = _tokenService.CreateToken(userLogin);

                return objToken.Result;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }


        /// <summary>
        /// Realiza o Refresh no token de Acesso
        /// </summary>
        /// <param name="tokenModel">Objeto de Negocio de Token</param>
        /// <returns>Informações do Token de Acesso</returns>
        public async Task<TokenOut> RefreshToken(TokenOut tokenModel)
        {
            try
            {
                string? accessToken = tokenModel.AccessToken;
                string? refreshToken = tokenModel.RefreshToken;

                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken).Result ?? throw new BusinessLogicCustomException("Invalid access token/refresh token");
                
                var user = await GetUserLogged();

                if (user == null || user.RefreshToken != refreshToken ||
                           user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    throw new BusinessLogicCustomException("Invalid access token/refresh token");
                }

                var objToken = _tokenService.CreateTokenExpired(principal.Claims.ToList(), user).Result;

                return objToken;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Revoga o Token de um usuario
        /// </summary>
        /// <param name="userId">Id do usuario</param>
        /// <returns></returns>
        public async Task<bool> Revoke(int userId)
        {
            try
            {
                var objUser = await GetUserByIdAsync(userId);

                if (objUser == null)
                {
                    throw new BusinessLogicCustomException(string.Format("{0}", OcPegResource.INVALID_USERNAME_PASSWORD));
                }

                objUser.RefreshToken = "";

                await UpdateAccount(objUser);

                return true;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Revoga o Token de Todos os Usuarios
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RevokeAll()
        {
            try
            {
                var users = GetUsersAsync().Result;

                foreach (var user in users)
                {
                    user.RefreshToken = "";

                    await UpdateAccount(user);
                }

                return true;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza as informações de Usuário
        /// </summary>
        /// <param name="user">Objeto de Negocio de Usuario</param>
        /// <returns></returns>
        public async Task<bool> UpdateAccount(User user)
        {
            try
            {
                if (user.PasswordHash != null)
                {
                    string token = await _accountRepository.GeneratePasswordResetTokenAsync(user);
                    await _accountRepository.ResetPasswordAsync(user, token, user.PasswordHash);
                }

                bool ret = await _userRepository.Update(user);

                return ret;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Verifica se um usuário existe
        /// </summary>
        /// <param name="userName">UserName do usuario</param>
        /// <returns></returns>
        public async Task<bool> UserExists(string email)
        {
            try
            {
                bool ret = await _userRepository.ExistActiveUserWithEmail(email);

                if (!ret)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém dados do Usuário pelo Id
        /// </summary>
        /// <param name="id">Id do usuário</param>
        /// <returns>Objeto de negocio UserDto</returns>
        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetById(id);

                return user;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém Todos os Usuários
        /// </summary>
        /// <returns>Lista de Objeto de negocio User</returns>
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                List<User> users = await _userRepository.GetAll();

                return users;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém dados do Usuário pelo Id
        /// </summary>
        /// <returns>Objeto de negocio UserDto</returns>
        public async Task<User> GetUserMain()
        {
            try
            {
                var user = await _accountRepository.GetAdm();

                return user;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="userDto">Objeto de Negocio de Usuário</param>
        /// <returns></returns>
        public async Task<User> GetUserLogged()
        {
            try
            {
                //Obtém o usuário pelo token
                Guid userIdent = _loggedUser.UserToken();

                User user = await _userRepository.GetByIdentifier(userIdent);

                return user;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }


        /// <summary>
        /// Checa a senha de um usário
        /// </summary>
        /// <param name="userName">Objeto de Login</param>
        /// <returns></returns>
        private static bool CheckUserPasswordAsync(string password, string loginPassword)
        {
            try
            {
                return password.Equals(loginPassword, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string GeneratePasswordHash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a hexadecimal string.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
