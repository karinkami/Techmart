namespace TechMart.Api.Services;

public sealed class UserEventMessage
{
    public int UserId { get; init; }
    public string EventType { get; init; } = string.Empty;
    public int? ProductId { get; init; }
    public int? Quantity { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
