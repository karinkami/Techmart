using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMart.Api.Data;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }
}

