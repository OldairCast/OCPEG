using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.Api.Controllers;
using OCPEG.API.Filters;
using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.OpenAI;
using OCPEG.Domain.BusinessObject;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using static OCPEG.Framework.Enums;
using System.Text.Json;

namespace OCPEG.API.Controllers
{
    /// <summary>
    /// Classe que faz acesso ao ChatGPT
    /// </summary>
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class OpenAIController : ControllerBaseLocal
    {
        private readonly IOpenAIService _openAIService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openAIService"></param>
        /// <param name="mapper"></param>
        public OpenAIController(IOpenAIService openAIService, IMapper mapper) : base(mapper)
        {
            _openAIService = openAIService;
        }

        /// <summary>
        /// Gera uma lista dereceita conforme parametros passsados
        /// </summary>
        /// <returns>Lista de Objeto de negocio ProductDto</returns>
        [AcceptVerbs("Post")]
        [Route("GenerateRecipe")]
        public async Task<IActionResult> GenerateRecipe(GenerateRecipeRequest request)
        {
            try
            {
                /* NOSONAR 
                  {
                    "ingredients": [
                        "2 tomates",
                        "1 ovo",
                        "1 cebola",
                        "1kg macarrão"
                    ]
                } 
                */

                NLogManager.LogInfo(Constants.Inicio);

                var result = await _openAIService.Execute(request);
                ResponseResult<GenerateRecipeResponse> objResult = new ResponseResult<GenerateRecipeResponse>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<GenerateRecipeResponse> objResult = new ResponseResult<GenerateRecipeResponse>
                {
                    Exchange = new GenerateRecipeResponse(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<GenerateRecipeResponse> objResult = new ResponseResult<GenerateRecipeResponse>
                {
                    Exchange = new GenerateRecipeResponse(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<GenerateRecipeResponse> objResult = new ResponseResult<GenerateRecipeResponse>
                {
                    Exchange = new GenerateRecipeResponse(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

    }
}
