using OCPEG.Domain.Dto;

namespace OCPEG.Application.Communication.Responses
{
    public class GenerateRecipeResponse
    {
        public string Title { get; set; } = string.Empty;
        public IList<string> Ingredients { get; set; } = [];
        public IList<GeneratedRecipeInstructionDto> Instructions { get; set; } = [];
        public CookingTime CookingTime { get; set; }
        public Difficulty Difficulty { get; set; }

    }


}
