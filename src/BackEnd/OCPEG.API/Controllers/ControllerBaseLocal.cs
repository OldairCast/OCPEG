using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerBaseLocal : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public ControllerBaseLocal(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Tratamento de Retorno para Exceções de validação
        /// </summary>
        /// <param name="ex">Mensagem de erro</param>
        /// <returns></returns>
        internal static string OCPegBadRequest(Exception ex)
        {
            return  $"OCPeg Validation: {ex.Message}";
        }

        /// <summary>
        /// Tratamento de Retorno para Exceções de sistema
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        internal static string OCPegInternalServerError(Exception ex)
        {
            return $"OCPeg Error. Erro: {ex.Message}";
        }
    }
}


