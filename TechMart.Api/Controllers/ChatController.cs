using Microsoft.AspNetCore.Mvc;
using TechMart.Api.DTOs;
using TechMart.Api.Services;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IMlServiceClient _mlServiceClient;

    public ChatController(IMlServiceClient mlServiceClient)
    {
        _mlServiceClient = mlServiceClient;
    }

    [HttpPost]
    public async Task<ActionResult> Ask([FromBody] ChatRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new { message = "Текст сообщения обязателен" });
        }

        try
        {
            var result = await _mlServiceClient.AskChatAsync(request.Text, cancellationToken);
            return Ok(new { reply = result.Reply, confidence = result.Confidence, intent = result.Intent });
        }
        catch
        {
            return Ok(new
            {
                reply = "Извините, я не понял. Попробуйте переформулировать",
                confidence = 0.0,
                intent = "fallback"
            });
        }
    }
}
