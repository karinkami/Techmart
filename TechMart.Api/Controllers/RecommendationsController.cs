using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechMart.Api.Data;
using TechMart.Api.DTOs;
using TechMart.Api.Services;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecommendationsController : ControllerBase
{
    private readonly IMlServiceClient _mlServiceClient;
    private readonly AppDbContext _context;

    public RecommendationsController(IMlServiceClient mlServiceClient, AppDbContext context)
    {
        _mlServiceClient = mlServiceClient;
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> GetRecommendations([FromBody] RecommendationsRequestDto request, CancellationToken cancellationToken)
    {
        var userId = request.UserId ?? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var limit = request.Limit <= 0 ? 5 : Math.Min(request.Limit, 20);
        if (userId == 0)
        {
            return Unauthorized(new { message = "Пользователь не определен" });
        }

        try
        {
            var recommendations = await _mlServiceClient.GetRecommendationsAsync(userId, limit, cancellationToken);
            var recommendedIds = recommendations.Select(x => x.ProductId).Distinct().ToList();
            if (recommendedIds.Count == 0)
            {
                return Ok(await GetPopularProductsAsync(limit, "popular-fallback"));
            }

            var products = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Manufacturer)
                .Where(x => recommendedIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var reasonById = recommendations.GroupBy(x => x.ProductId).ToDictionary(x => x.Key, x => x.First().Reason);
            var ordered = recommendedIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .Take(limit)
                .Select(p => new
                {
                    id = p!.Id,
                    title = p.Title,
                    description = p.Description,
                    price = p.Price,
                    imageUrl = p.ImageUrl,
                    reason = string.IsNullOrWhiteSpace(reasonById.GetValueOrDefault(p.Id))
                        ? "Потому что вы интересовались похожими товарами"
                        : reasonById[p.Id]
                });

            return Ok(new { source = "ml", items = ordered });
        }
        catch
        {
            return Ok(await GetPopularProductsAsync(limit, "popular-fallback"));
        }
    }

    private async Task<object> GetPopularProductsAsync(int limit, string source)
    {
        var popularIds = await _context.OrderItems
            .GroupBy(x => x.ProductId)
            .OrderByDescending(x => x.Sum(i => i.Quantity))
            .Select(x => x.Key)
            .Take(limit)
            .ToListAsync();

        if (popularIds.Count == 0)
        {
            popularIds = await _context.Products
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .Take(limit)
                .ToListAsync();
        }

        var products = await _context.Products
            .Where(x => popularIds.Contains(x.Id))
            .ToListAsync();

        var items = popularIds
            .Select(id => products.FirstOrDefault(p => p.Id == id))
            .Where(x => x != null)
            .Select(x => new
            {
                id = x!.Id,
                title = x.Title,
                description = x.Description,
                price = x.Price,
                imageUrl = x.ImageUrl,
                reason = "Популярный товар среди покупателей"
            });

        return new { source, items };
    }
}
