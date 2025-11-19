using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMart.Api.Data;

namespace TechMart.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ManufacturersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetManufacturers()
    {
        return await _context.Manufacturers.ToListAsync();
    }
}

