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
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
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

