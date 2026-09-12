using OCPEG.Common.TestUtilities.Requests;
using System.Net;
using System.Net.Http.Json;

namespace OCPEG.WebApi.Test
{
    public class UserApiInMemoryTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly string method = "user";

        public UserApiInMemoryTest(CustomWebApplicationFactory factory) => _httpClient = factory.CreateClient();

        
        [Fact]
        public async Task InsertUserTest()
        {
            var request = RequestUserTest.BuildDto();

            var cancellationToken = TestContext.Current.CancellationToken;

            var response = await _httpClient.PostAsJsonAsync(
                method + "/Create",
                request,
                cancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    }
}
