namespace TechMart.Api.Services;

public interface IMlServiceClient
{
    Task<IReadOnlyList<RecommendationResult>> GetRecommendationsAsync(int userId, int limit, CancellationToken cancellationToken = default);
    Task<ChatResult> AskChatAsync(string text, CancellationToken cancellationToken = default);
    Task TrackEventAsync(int userId, string eventType, int? productId, int? quantity, CancellationToken cancellationToken = default);
}

public sealed record RecommendationResult(int ProductId, string Reason);
public sealed record ChatResult(string Reply, double Confidence, string Intent);
