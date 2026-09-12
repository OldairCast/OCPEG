using System.Security.Claims;

namespace OCPEG.API.Extensions
{
    /// <summary>
    /// Classe de Extensão que retorna as Claims
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Retorna Claims pelo Nome do Usuário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string? GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        /// <summary>
        /// Retorna Claims pelo Id do Usuaário
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var x = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return x != null ? int.Parse(x) : 0;
        }

    }
}
