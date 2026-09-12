
namespace OCPEG.Domain.Dto
{
    public record GeneratedRecipeDto
    {
        public string Title { get; init; } = string.Empty;
        public IList<string> Ingredients { get; init; } = [];
        public IList<GeneratedRecipeInstructionDto> Instructions { get; init; } = [];
        public CookingTime CookingTime { get; init; }
    }

    public record GeneratedRecipeInstructionDto
    {
        public int Step { get; init; }
        public string Text { get; init; } = string.Empty;
    }

    public enum CookingTime
    {
        Less_10_Minutes = 0,
        Between_10_30_Minutes = 1,
        Betwenn_30_60_Minutes = 2,
        Greather_60_Minutes = 3
    }

    public enum Difficulty
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
