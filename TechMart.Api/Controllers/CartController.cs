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
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var cartItems = await _context.ShoppingCart
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ThenInclude(p => p!.Category)
            .Include(c => c.Product)
            .ThenInclude(p => p!.Manufacturer)
            .ToListAsync();

        var result = cartItems.Select(c => new CartItemDto
        {
            Id = c.Id,
            ProductId = c.ProductId,
            Quantity = c.Quantity,
            Product = c.Product != null ? new ProductDto
            {
                Id = c.Product.Id,
                Title = c.Product.Title,
                Description = c.Product.Description,
                Price = c.Product.Price,
                ImageUrl = c.Product.ImageUrl
            } : null
        }).ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<CartItemDto>> AddToCart([FromBody] AddToCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        // Проверяем существование товара
        var product = await _context.Products.FindAsync(dto.ProductId);
        if (product == null)
        {
            return NotFound(new { message = "Товар не найден" });
        }

        // Проверяем, есть ли уже этот товар в корзине
        var existingItem = await _context.ShoppingCart
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        if (existingItem != null)
        {
            // Увеличиваем количество
            existingItem.Quantity += dto.Quantity;
        }
        else
        {
            // Создаем новый элемент корзины
            existingItem = new ShoppingCart
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            _context.ShoppingCart.Add(existingItem);
        }

        await _context.SaveChangesAsync();

        // Загружаем товар для ответа
        await _context.Entry(existingItem)
            .Reference(c => c.Product)
            .LoadAsync();

        var result = new CartItemDto
        {
            Id = existingItem.Id,
            ProductId = existingItem.ProductId,
            Quantity = existingItem.Quantity,
            Product = existingItem.Product != null ? new ProductDto
            {
                Id = existingItem.Product.Id,
                Title = existingItem.Product.Title,
                Description = existingItem.Product.Description,
                Price = existingItem.Product.Price,
                ImageUrl = existingItem.Product.ImageUrl
            } : null
        };

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCartItem(int id, [FromBody] AddToCartDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var cartItem = await _context.ShoppingCart
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Элемент корзины не найден" });
        }

        cartItem.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveFromCart(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var cartItem = await _context.ShoppingCart
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cartItem == null)
        {
            return NotFound(new { message = "Элемент корзины не найден" });
        }

        _context.ShoppingCart.Remove(cartItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var cartItems = await _context.ShoppingCart
            .Where(c => c.UserId == userId)
            .ToListAsync();

        _context.ShoppingCart.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

