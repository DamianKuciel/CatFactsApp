using System.Net;
using System.Net.Http.Json;
using CatFactsApp.Models;
using CatFactsApp.Services;
using Xunit;

namespace CatFactsApp.Tests
{
    public class CatFactClientTests
    {
        [Fact]
        public async Task GetFactAsync_ReturnsValidResponse()
        {
            var expectedFact = new CatFactResponse("Test cat fact", 13);
            var handlerMock = new HttpMessageHandlerMock(expectedFact);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://catfact.ninja/") };
            var client = new CatFactClient(httpClient);

            var result = await client.GetFactAsync();

            Assert.NotNull(result);
            Assert.Equal("Test cat fact", result.Fact);
            Assert.Equal(13, result.Length);
        }
    }

    public class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly CatFactResponse _response;
        public HttpMessageHandlerMock(CatFactResponse response) => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_response);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            return Task.FromResult(httpResponse);
        }


    }
}