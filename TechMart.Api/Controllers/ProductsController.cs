using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMart.Api.Data;
using TechMart.Api.Models;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        [FromQuery] int? categoryId,
        [FromQuery] int? manufacturerId,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir)
    {
        try
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (manufacturerId.HasValue)
            {
                query = query.Where(p => p.ManufacturerId == manufacturerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.ILike(p.Title, pattern) ||
                    EF.Functions.ILike(p.Description, pattern));
            }

            var normalizedSortBy = (sortBy ?? "id").Trim().ToLowerInvariant();
            var isDesc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            query = normalizedSortBy switch
            {
                "price" => isDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "title" => isDesc ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
                _ => isDesc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
            };

            var products = await query
                .ToListAsync();
            
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка при загрузке товаров", message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return product;
    }
}

