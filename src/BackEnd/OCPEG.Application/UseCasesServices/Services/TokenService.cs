using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Entities;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class TokenService : ITokenService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private const string jwtNoConfig = "JWT key is not configured.";

        public TokenService(IConfiguration config,
                            IAccountRepository accountRepository,
                            IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _config = config;
        }

        public async Task<TokenOut> CreateToken(User user)
        {
            try
            {
                var signingCredentials = GetSigningCredentials();
                var claims = await GetClaims(user);
                var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
                var refreshToken = GenerateRefreshToken();
                var RefreshTokenExpiryTime = RefreshTokenValidityInMinutes();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = RefreshTokenExpiryTime;

                //Atualiza no banco
                await _accountRepository.UpdateToken(user);

                TokenOut tokenOut = new TokenOut
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = RefreshTokenExpiryTime,
                    UserName = user.Name == null ? string.Empty : user.Name,
                    UserIdentifier = user.UserIdentifier.ToString()
                };

                return tokenOut;

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

        public async Task<TokenOut> CreateTokenExpired(List<Claim> authClaims, User user)
        {
            try
            {
                var signingCredentials = GetSigningCredentials();
                var claims = authClaims;
                var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
                var refreshToken = GenerateRefreshToken();
                var RefreshTokenExpiryTime = RefreshTokenValidityInMinutes();

                await _userRepository.Update(user);

                TokenOut tokenOut = new TokenOut
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiryTime = RefreshTokenExpiryTime
                };

                return tokenOut;

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
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token)
        {
            try
            {
                var jwtConfig = _config.GetSection(Constants.JwtConfig) ??
                    throw new BusinessLogicCustomException(jwtNoConfig);

                var secretKey = jwtConfig[Constants.SecretKey] ?? throw new BusinessLogicCustomException(jwtNoConfig);
                var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secret,
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                          !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                         StringComparison.InvariantCultureIgnoreCase))
                    throw new OcException("Invalid token");

                return await Task.Run(() => principal);

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
        /// 
        /// </summary>
        /// <returns></returns>
        private SigningCredentials GetSigningCredentials()
        {
            var jwtConfig = _config.GetSection(Constants.JwtConfig) ??
                throw new BusinessLogicCustomException(jwtNoConfig);

            var secretKey = jwtConfig[Constants.SecretKey] ?? throw new BusinessLogicCustomException(jwtNoConfig);
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<List<Claim>> GetClaims(User user)
        {

            //claims - afirmações(sobre voce)
            //Cria as claims baseadas no usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name == null ? string.Empty : user.Name),
                new Claim(ClaimTypes.Sid, user.UserIdentifier.ToString())
            };

            //pega no banco as roles(responsabilidades) do usuário
            var roles = await _accountRepository.GetRoles(user.Id.ToString());

            //adiciona para dentro de claims as roles que acabou de pegar e coloca no claimType
            foreach (Role role in roles)
            {
                claims.Add(new Claim(role.RoleId!, role.Nome!));
            }
            return claims;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signingCredentials"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _config.GetSection(Constants.JwtConfig);
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            //expires: DateTime.Now.AddDays(1),
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["TokenValidityInMinutes"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private DateTime RefreshTokenValidityInMinutes()
        {
            var jwtConfig = _config.GetSection(Constants.JwtConfig);
            _ = int.TryParse(jwtConfig["RefreshTokenValidityInMinutes"],
            out int refreshTokenValidityInMinutes);

            return DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);
        }

    }
}