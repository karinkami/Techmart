using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechMart.Api.Data;
using TechMart.Api.DTOs;
using TechMart.Api.Models;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReturnsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReturnsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("mine")]
    public async Task<ActionResult> GetMyReturns(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0)
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        var items = await _context.ReturnRequests
            .Where(r => r.UserId == userId)
            .Select(r => new
            {
                id = r.Id,
                orderId = r.OrderId,
                status = r.Status,
                createdAt = r.CreatedAt
            })
            .OrderByDescending(r => r.createdAt)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> CreateReturn([FromBody] CreateReturnRequestDto request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0)
        {
            return Unauthorized(new { message = "Не удалось определить пользователя" });
        }

        if (request.OrderId <= 0 || string.IsNullOrWhiteSpace(request.Reason) || string.IsNullOrWhiteSpace(request.ContactPhone))
        {
            return BadRequest(new { message = "Заполните обязательные поля заявки на возврат" });
        }

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == userId, cancellationToken);
        if (order is null)
        {
            return NotFound(new { message = "Заказ не найден" });
        }

        var normalizedStatus = (order.Status ?? string.Empty).Trim().ToLowerInvariant();
        var canReturn = normalizedStatus is "delivered" or "completed" or "done";
        if (!canReturn)
        {
            return BadRequest(new { message = "Возврат доступен только для выполненных/доставленных заказов" });
        }

        var activeStatuses = new[] { "pending", "in_review", "approved" };
        var exists = await _context.ReturnRequests.AnyAsync(
            r => r.UserId == userId && r.OrderId == request.OrderId && activeStatuses.Contains(r.Status),
            cancellationToken);
        if (exists)
        {
            return Conflict(new { message = "По этому заказу уже есть активная заявка на возврат" });
        }

        var returnRequest = new ReturnRequest
        {
            UserId = userId,
            OrderId = request.OrderId,
            Reason = request.Reason.Trim(),
            Details = string.IsNullOrWhiteSpace(request.Details) ? null : request.Details.Trim(),
            PreferredResolution = string.IsNullOrWhiteSpace(request.PreferredResolution) ? "refund" : request.PreferredResolution.Trim(),
            ContactPhone = request.ContactPhone.Trim(),
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.ReturnRequests.Add(returnRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            id = returnRequest.Id,
            orderId = returnRequest.OrderId,
            status = returnRequest.Status,
            createdAt = returnRequest.CreatedAt
        });
    }
}
