using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Codel_Cloud_Native.Tests;

public class CodeleApiClientTests
{
    [Fact]
    public async Task GetSampleDataAsync_ReturnsItems()
    {
        // Arrange
        var responses = new[] { new { Answer = "APPLE" }, new { Answer = "BERRY" } };
        var handler = new TestMessageHandler(req =>
        {
            var json = JsonSerializer.Serialize(responses);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(responses)
            };
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.local")
        };

        var client = new Codel_Cloud_Native.Web.CodeleApiClient(httpClient);

        // Act
        var result = await client.GetSampleDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal("APPLE", result[0].Answer);
    }

    [Fact]
    public async Task GetSampleDataAsync_RetriesOnTransientFailure()
    {
        // Arrange: first two attempts fail with 500, third succeeds
        int attempt = 0;
        var handler = new TestMessageHandler(req =>
        {
            attempt++;
            if (attempt < 3)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            var responses = new[] { new { Answer = "CHERRY" } };
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(responses)
            };
        });

        // Build services with typed client + retry policy and configure the primary handler to our test handler
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        var retryPolicy = Polly.Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(10));

        services.AddHttpClient<Codel_Cloud_Native.Web.CodeleApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://example.local");
        })
        .ConfigurePrimaryHttpMessageHandler(() => handler)
        .AddPolicyHandler(retryPolicy);

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<Codel_Cloud_Native.Web.CodeleApiClient>();

        // Act
        var result = await client.GetSampleDataAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("CHERRY", result[0].Answer);
        Assert.Equal(3, attempt);
    }
}

// Minimal test HttpMessageHandler helper
internal class TestMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public TestMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder ?? throw new ArgumentNullException(nameof(responder));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responder(request));
    }
}
