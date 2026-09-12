using Microsoft.AspNetCore.Mvc.Testing;
using OCPEG.Common.TestUtilities.Repositories;
using OCPEG.Common.TestUtilities.Requests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OCPEG.WebApi.Test
{
    /*Precisa informar que essa classe é uma classe especial porque por baixo dos panos é preciso que.Net libere alguns acessos
    Para marcar que essa classe é um teste de integração marca que essa classe irá implementar uma classe "IClassFixture" que precisa receber um servidor.
    Cada classe vai executar dentro de um servidor.
    O.Net fornece um servidor para ser utilizado.
    IClassFixture<WebApplicationFactory>
    Instalar Microsoft.AspNetCore.Mvc.Testing
    Precisa informar para o servidor o que ele vai executar, 
    para isso vai na primeira classe a ser executada que é o program e cria um construtor lá
    O servidor é onde ele irá executar a nossa api para fazermos os nossos testes
    */

    public class UserApiTest: IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _httpClient;
        private readonly string method = "user";

        public UserApiTest(WebApplicationFactory<Program> factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task InsertUserTest()
        {
            LoginBuilder login = new LoginBuilder(_httpClient);
            string accessToken = await login.LoginTest();


            var request = RequestUserTest.BuildDto();
            request.RefreshToken = accessToken;


            var cancellationToken = TestContext.Current.CancellationToken;

            var response = await _httpClient.PostAsJsonAsync(
                method + "/Create",
                request,
                cancellationToken);

            await using var responseBody = await response.Content.ReadAsStreamAsync(cancellationToken);

            var responseData = await JsonDocument.ParseAsync(responseBody, cancellationToken: cancellationToken);

            var x = responseData.RootElement.GetProperty("exchange");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }


}
