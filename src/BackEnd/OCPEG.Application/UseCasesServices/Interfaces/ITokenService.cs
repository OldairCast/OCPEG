using OCPEG.Domain.BusinessObject;
using System.Security.Claims;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface ITokenService
    {
        Task<TokenOut> CreateToken(User user);

        Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token);

        Task<TokenOut> CreateTokenExpired(List<Claim> authClaims, User user);
    }
}
