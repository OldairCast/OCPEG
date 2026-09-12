using static OCPEG.Framework.Enums;

namespace OCPEG.Framework
{
    /// <summary>
    /// Retorno padrão de metodos WS no Exceller
    /// </summary>
    public class ResponseResult : IResponse
    {
        private ResponseResultStatus _status;
        private bool _set;

        public ResponseResult()
        {
            Message = string.Empty;
        }

        /// <summary>
        /// Status da Operação
        /// </summary>
        public ResponseResultStatus Status
        {
            get
            {
                if (_set)
                    return _status;

                return string.IsNullOrEmpty(Message)
                            ? ResponseResultStatus.Success
                            : ResponseResultStatus.Error;
            }
            set
            {
                _set = true;
                _status = value;
            }
        }

        /// <summary>
        /// Mensagem de resultado da operação no caso de Insucesso
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Codigo de erro no caso de Insucesso
        /// </summary>
        public int Code { get; set; }

        public bool Error
        {
            get
            {
                return
                    Status == ResponseResultStatus.Error;
            }
        }
    }

    /// <summary>
    /// Padrão genêrico de retorno
    /// </summary>
    /// <typeparam name="T">Tipo da Resposta</typeparam>
    public class ResponseResult<T> : ResponseResult, IResponse<T>
    {

        public ResponseResult()
        {
            Message = string.Empty;

        }

        public ResponseResult(ResponseResultStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Conteudo da resposta
        /// </summary>
        public required T Exchange { get; set; }
    }


}
