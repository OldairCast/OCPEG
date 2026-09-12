using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;

namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.OpenAI
{
    public interface IOpenAIService
    {
        Task<GenerateRecipeResponse> Execute(GenerateRecipeRequest request);
    }
}
