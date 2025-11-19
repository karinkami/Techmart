namespace TechMart.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int ManufacturerId { get; set; }
    public string? ImageUrl { get; set; }
    
    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
}

