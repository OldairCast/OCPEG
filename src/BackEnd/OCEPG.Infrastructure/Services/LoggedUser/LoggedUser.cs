using OCEPG.Infrastructure.DataAccess.Base;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OCEPG.Infrastructure.Services.LoggedUser
{
    public class LoggedUser : ILoggedUser
    {
        private readonly ITokenProvider _tokenProvider;

        public LoggedUser(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        public Guid UserToken()
        {
            var token = _tokenProvider.Value();

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            var identifier = jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

            var userIdentifier = Guid.Parse(identifier);

            return userIdentifier;
        }

    }

}
