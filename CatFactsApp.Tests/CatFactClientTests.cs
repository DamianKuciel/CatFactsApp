using CatFactsApp.Models;
using CatFactsApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace CatFactsApp.Tests
{
    public class CatFactClientTests
    {
        [Fact]
        public async Task GetFactAsync_ReturnsValidResponse()
        {
            var expectedFact = new CatFactResponse("Baking chocolate is dangerous for cats.", 45);
            var handlerMock = new HttpMessageHandlerMock(expectedFact);
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://catfact.ninja/") };
            var client = new CatFactClient(httpClient);

            var result = await client.GetFactAsync();

            Assert.NotNull(result);
            Assert.Equal("Baking chocolate is dangerous for cats.", result.Fact);
            Assert.Equal(45, result.Length);
        }

        [Fact]
        public async Task GetFactAsync_ReturnsNull_WhenApiReturnsNull()
        {
            var handlerMock = new HttpMessageHandlerNullMock();
            var httpClient = new HttpClient(handlerMock) { BaseAddress = new Uri("https://catfact.ninja/") };
            var client = new CatFactClient(httpClient);

            var result = await client.GetFactAsync();
            Assert.Null(result);
        }

        [Fact]
        public async Task GetFactAsync_Throws_WhenResponseIsMalformed()
        {
            var handlerMock = new HttpMessageHandlerMalformedJsonMock();
            var httpClient = new HttpClient(handlerMock)
            {
                BaseAddress = new Uri("https://catfact.ninja/")
            };

            var client = new CatFactClient(httpClient);

            await Assert.ThrowsAsync<System.Text.Json.JsonException>(
                () => client.GetFactAsync());
        }

        [Fact]
        public async Task GetFactAsync_PropagatesCancellation()
        {
            var handlerMock = new HttpMessageHandlerMock(
                new CatFactResponse("test", 4));

            var httpClient = new HttpClient(handlerMock)
            {
                BaseAddress = new Uri("https://catfact.ninja/")
            };

            var client = new CatFactClient(httpClient);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(
                () => client.GetFactAsync(cts.Token));
        }

        [Fact]
        public async Task GetFactAsync_RetriesTransientFailure()
        {
            var handlerMock = new HttpMessageHandlerRetryMock();

            var services = new ServiceCollection();

            services.AddHttpClient<ICatFactClient, CatFactClient>(client =>
            {
                client.BaseAddress = new Uri("https://catfact.ninja/");
            })
            .ConfigurePrimaryHttpMessageHandler(() => handlerMock)
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromMilliseconds(10);
            });

            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<ICatFactClient>();

            var result = await client.GetFactAsync();

            Assert.NotNull(result);
            Assert.Equal("After 2 retries, a wild fact appears!", result.Fact);
            Assert.Equal(3, handlerMock.RequestCount);
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

    public class HttpMessageHandlerNullMock : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
            };
            return Task.FromResult(httpResponse);
        }
    }

    public class HttpMessageHandlerMalformedJsonMock : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{ invalid json }",
                    System.Text.Encoding.UTF8,
                    "application/json")
            };

            return Task.FromResult(httpResponse);
        }
    }

    public class HttpMessageHandlerRetryMock : HttpMessageHandler
    {
        public int RequestCount { get; private set; } = 0;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;

            if (RequestCount <= 2)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }

            var response = new CatFactResponse("After 2 retries, a wild fact appears!", 37);
            var json = System.Text.Json.JsonSerializer.Serialize(response);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            });
        }
    }



}