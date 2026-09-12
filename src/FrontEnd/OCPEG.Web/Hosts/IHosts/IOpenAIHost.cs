using OCPEG.Framework;

namespace OCPEG.Web.Hosts.IHosts
{
    public interface IOpenAIHost
    {
        Task<ResponseResult<string>> ExecuteMain(string token, string requestJson);
    }
}
