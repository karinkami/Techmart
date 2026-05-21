using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace TechMart.Api.Services;

public sealed class RedisUserEventPublisher : IUserEventPublisher
{
    private readonly RedisOptions _options;
    private readonly ILogger<RedisUserEventPublisher> _logger;
    private readonly IConnectionMultiplexer? _redis;

    public RedisUserEventPublisher(IOptions<RedisOptions> options, ILogger<RedisUserEventPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!_options.Enabled || string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            return;
        }

        try
        {
            _redis = ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis init failed, fallback to ML HTTP tracking will be used.");
        }
    }

    public async Task<bool> PublishAsync(UserEventMessage message, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || _redis is null || string.IsNullOrWhiteSpace(_options.QueueKey))
        {
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var payload = JsonSerializer.Serialize(new
        {
            user_id = message.UserId,
            event_type = message.EventType,
            product_id = message.ProductId,
            quantity = message.Quantity,
            timestamp = message.Timestamp
        });

        try
        {
            var db = _redis.GetDatabase();
            await db.ListRightPushAsync(_options.QueueKey, payload);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis publish failed for event {EventType}.", message.EventType);
            return false;
        }
    }
}
