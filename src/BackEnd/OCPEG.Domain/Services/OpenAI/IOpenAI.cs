using OCPEG.Domain.Dto;

namespace OCPEG.Domain.Services.OpenAI
{
    public interface IOpenAI
    {
        Task<GeneratedRecipeDto> GeneratedRecipe(IList<string> questions);
    }
}
