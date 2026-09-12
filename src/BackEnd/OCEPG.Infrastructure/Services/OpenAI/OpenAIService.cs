using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.OpenAI;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Domain.Services.OpenAI;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using System.Text.Json;
using static OCPEG.Framework.Enums;

namespace OCEPG.Infrastructure.Services.OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IOpenAI _generator;
        public const int MAXIMUM_INGREDIENTS_GENERATE_RECIPE = 5;
        public const int MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES = 10;

        public OpenAIService(IOpenAI generator)
        {
            _generator = generator;
        }

        public async Task<string> ExecuteMain(GenerateRecipeRequest request)
        {
            try
            {
                GenerateRecipeResponse response = await Execute(request);

                string jsonString = "";

                if (response != null)
                {
                    jsonString = JsonSerializer.Serialize(response);
                }

                return jsonString;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }

        }

        public async Task<GenerateRecipeResponse> Execute(GenerateRecipeRequest request)
        {
            Validate(request);

            var recipe = await _generator.GeneratedRecipe(request.Ingredients);

            GenerateRecipeResponse recipeResponse = new GenerateRecipeResponse
            {
                Title = recipe.Title,
                Ingredients = recipe.Ingredients,
                CookingTime = recipe.CookingTime,
                Instructions = recipe.Instructions.Select(c => new GeneratedRecipeInstructionDto
                {
                    Step = c.Step,
                    Text = c.Text,
                }).ToList(),
                Difficulty = Difficulty.Low
            };

            return recipeResponse;

        }

        private static void Validate(GenerateRecipeRequest request)
        {
            var maximum_number_ingredients = MAXIMUM_INGREDIENTS_GENERATE_RECIPE;

            if (request.Ingredients.Count < 1 || request.Ingredients.Count > maximum_number_ingredients)
            {
                throw new BusinessLogicCustomException("Numero de Ingredientes Inválido");
            }
        }
    }
}
