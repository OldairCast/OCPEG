using static OCPEG.Framework.Enums;

namespace OCPEG.Framework
{
    /// <summary>
    /// Interface de retorno
    /// </summary>
    public interface IResponse
    {
        ResponseResultStatus Status { get; }
        string Message { get; set; }
        int Code { get; set; }
    }

    public interface IResponse<T> : IResponse
    {
        T Exchange { get; set; }
    }

}
