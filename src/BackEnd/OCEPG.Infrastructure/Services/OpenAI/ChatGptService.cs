using OCPEG.Domain.Dto;
using OCPEG.Domain.Services.OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCEPG.Infrastructure.Services.OpenAI
{
    public class ChatGptService: IOpenAI
    {
        private readonly ChatClient _chatClient;
        private readonly string STARTING_GENERATE_RECIPE = "Você é um chef que conhece muitas receitas saborosas. Quando o usuário lhe fornecer uma lista de ingredientes separados por ';', você responderá com apenas uma receita que seja fácil de criar com os ingredientes informados.\r\n\r\nVocê criará uma receita com a seguinte estrutura e ordem: \r\n\r\n[Nome da Receita]\r\n\r\n[tempo necessário para prepará-la, em que 0 significa menos de 10 minutos, 1 significa entre 10 e 30 minutos, 2 significa entre 30 e 60 minutos ou 3 significa mais de 60 minutos]\r\n\r\n[Lista de ingredientes fornecidos e os adicionados por você separados por ';']\r\n\r\n[Instruções passo a passo para o preparo da receita em apenas uma linha, em ordem e separados por '@']\r\n\r\nNão adicione mais texto do que o solicitado por favor.";

        public ChatGptService(ChatClient chatClient) => _chatClient = chatClient;



        public async Task<GeneratedRecipeDto> GeneratedRecipe(IList<string> questions)
        {
            var messages = new List<ChatMessage>
        {
            new SystemChatMessage(STARTING_GENERATE_RECIPE),
            new UserChatMessage(string.Join(";", questions))
        };

            var completion = await _chatClient.CompleteChatAsync(messages);

            var responseList = completion.Value.Content[0].Text
                .Split("\n")
                .Where(response => !string.IsNullOrEmpty(response.Trim()))
                .Select(item => item.Replace("[", "").Replace("]", ""))
                .ToList();

            var step = 1;

            return new GeneratedRecipeDto
            {
                Title = responseList[0],
                CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), responseList[1]),
                Ingredients = responseList[2].Split(";"),
                Instructions = responseList[3].Split("@").Select(instruction => new GeneratedRecipeInstructionDto
                {
                    Text = instruction.Trim(),
                    Step = step++
                }).ToList()
            };
        }

    }
}
