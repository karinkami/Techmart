namespace TechMart.Api.Services;

public interface IUserEventTracker
{
    Task TrackAsync(UserEventMessage message, CancellationToken cancellationToken = default);
}
