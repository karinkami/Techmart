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
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderDto orderDto)
    {
        try
        {
            // Получаем ID пользователя из токена
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new { message = "Не удалось определить пользователя" });
            }

            // Получаем товары из корзины пользователя
            var cartItems = await _context.ShoppingCart
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems == null || cartItems.Count == 0)
            {
                return BadRequest(new { message = "Корзина пуста" });
            }

            // Вычисляем общую сумму
            decimal totalAmount = cartItems.Sum(item => item.Product!.Price * item.Quantity);

            // Создаем заказ
            var order = new Order
            {
                UserId = userId,
                Name = orderDto.Name,
                Email = orderDto.Email,
                Address = orderDto.Address,
                Phone = orderDto.Phone,
                TotalAmount = totalAmount,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Создаем элементы заказа
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Price = cartItem.Product!.Price,
                    Quantity = cartItem.Quantity
                };
                _context.OrderItems.Add(orderItem);
            }

            // Очищаем корзину
            _context.ShoppingCart.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            // Загружаем заказ с элементами для возврата
            var createdOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == order.Id);

            return Ok(new { 
                id = createdOrder!.Id,
                totalAmount = createdOrder.TotalAmount,
                status = createdOrder.Status,
                createdAt = createdOrder.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка при создании заказа", message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new { message = "Не удалось определить пользователя" });
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Category)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p!.Manufacturer)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                Name = o.Name,
                Email = o.Email,
                Address = o.Address,
                Phone = o.Phone,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Product = oi.Product != null ? new ProductDto
                    {
                        Id = oi.Product.Id,
                        Title = oi.Product.Title,
                        Description = oi.Product.Description,
                        Price = oi.Product.Price,
                        ImageUrl = oi.Product.ImageUrl
                    } : null
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Ошибка при загрузке заказов", message = ex.Message });
        }
    }
}

