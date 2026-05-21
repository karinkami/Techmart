namespace TechMart.Api.Models;

public class ReturnRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string PreferredResolution { get; set; } = "refund";
    public string ContactPhone { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Order? Order { get; set; }
}
