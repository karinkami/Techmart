using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace TechMart.Api.Services;

public class MlServiceClient : IMlServiceClient
{
    private readonly HttpClient _httpClient;

    public MlServiceClient(HttpClient httpClient, IOptions<MlServiceOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/'));
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<IReadOnlyList<RecommendationResult>> GetRecommendationsAsync(int userId, int limit, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/recommend", new { user_id = userId, limit }, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<RecommendResponse>(cancellationToken: cancellationToken);
        var items = payload?.Recommendations?
            .Where(x => x.ProductId > 0)
            .Select(x => new RecommendationResult(x.ProductId, x.Reason ?? string.Empty))
            .ToList()
            ?? new List<RecommendationResult>();

        return items;
    }

    public async Task<ChatResult> AskChatAsync(string text, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/chat", new { text }, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ChatResult("Извините, я не понял. Попробуйте переформулировать", 0, "unknown");
        }

        var payload = JsonSerializer.Deserialize<ChatResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return new ChatResult(
            payload?.Reply ?? "Извините, я не понял. Попробуйте переформулировать",
            payload?.Confidence ?? 0,
            payload?.Intent ?? "unknown");
    }

    public async Task TrackEventAsync(int userId, string eventType, int? productId, int? quantity, CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsJsonAsync("/events", new
        {
            user_id = userId,
            event_type = eventType,
            product_id = productId,
            quantity = quantity,
            timestamp = DateTimeOffset.UtcNow
        }, cancellationToken);
    }

    private sealed class RecommendResponse
    {
        public List<RecommendItem>? Recommendations { get; set; }
    }

    private sealed class RecommendItem
    {
        [JsonPropertyName("product_id")]
        public int ProductId { get; set; }
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

    private sealed class ChatResponse
    {
        [JsonPropertyName("reply")]
        public string? Reply { get; set; }
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
        [JsonPropertyName("intent")]
        public string? Intent { get; set; }
    }
}
