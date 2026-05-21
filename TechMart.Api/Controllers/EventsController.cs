using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechMart.Api.DTOs;
using TechMart.Api.Services;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly IUserEventTracker _userEventTracker;

    public EventsController(IUserEventTracker userEventTracker)
    {
        _userEventTracker = userEventTracker;
    }

    [HttpPost("track")]
    public async Task<ActionResult> Track([FromBody] TrackEventDto request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0)
        {
            return Unauthorized(new { message = "Пользователь не определен" });
        }

        if (string.IsNullOrWhiteSpace(request.EventType))
        {
            return BadRequest(new { message = "EventType обязателен" });
        }

        try
        {
            await _userEventTracker.TrackAsync(new UserEventMessage
            {
                UserId = userId,
                EventType = request.EventType,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Timestamp = DateTime.UtcNow
            }, cancellationToken);
        }
        catch
        {
            // События best-effort: пользовательский flow не блокируем.
        }

        return Accepted();
    }
}
