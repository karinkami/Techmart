namespace TechMart.Api.Services;

public sealed class UserEventTracker : IUserEventTracker
{
    private readonly IUserEventPublisher _publisher;
    private readonly IMlServiceClient _mlServiceClient;

    public UserEventTracker(IUserEventPublisher publisher, IMlServiceClient mlServiceClient)
    {
        _publisher = publisher;
        _mlServiceClient = mlServiceClient;
    }

    public async Task TrackAsync(UserEventMessage message, CancellationToken cancellationToken = default)
    {
        var published = await _publisher.PublishAsync(message, cancellationToken);
        if (published)
        {
            return;
        }

        await _mlServiceClient.TrackEventAsync(
            message.UserId,
            message.EventType,
            message.ProductId,
            message.Quantity,
            cancellationToken);
    }
}
