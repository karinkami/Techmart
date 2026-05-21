namespace TechMart.Api.DTOs;

public class CreateReturnRequestDto
{
    public int OrderId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string PreferredResolution { get; set; } = "refund";
    public string ContactPhone { get; set; } = string.Empty;
}
