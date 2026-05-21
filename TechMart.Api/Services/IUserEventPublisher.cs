namespace TechMart.Api.Services;

public interface IUserEventPublisher
{
    Task<bool> PublishAsync(UserEventMessage message, CancellationToken cancellationToken = default);
}
