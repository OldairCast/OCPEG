using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using System.Net.Http.Json;

namespace OCPEG.Common.TestUtilities.Repositories
{
    public class LoginBuilder
    {
        private readonly HttpClient _httpClient;

        public LoginBuilder(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<string> LoginTest()
        {
            UserLoginDto userLogin = new UserLoginDto
            {
                Email = "ocpeg@ocpeg.com",
                Password = "ocpeg123"
            };

            var responseL = await _httpClient.PostAsJsonAsync("Account/Login", userLogin);
            var objResponse = await responseL.Content.ReadFromJsonAsync<ResponseResult<TokenOut>>();
            return objResponse!.Exchange.AccessToken;

            /* NOSONAR
            await using var responseBodyD = await responseL.Content.ReadAsStreamAsync();
            var responseDataD = await JsonDocument.ParseAsync(responseBodyD);
            var accessToken = responseDataD.RootElement.GetProperty("accessToken").GetString() != null ?
                responseDataD.RootElement.GetProperty("accessToken").GetString() : "";
            return accessToken!;
            */
        }
    }
}
