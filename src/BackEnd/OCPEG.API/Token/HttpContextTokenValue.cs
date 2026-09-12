using OCPEG.Domain.Security;

namespace OCPEG.API.Token;

/// <summary>
/// 
/// </summary>
public class HttpContextTokenValue : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contextAccessor"></param>
    public HttpContextTokenValue(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string Value()
    {
        var authentication = _contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

        return authentication["Bearer ".Length..].Trim();
    }
}
