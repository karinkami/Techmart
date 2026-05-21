namespace TechMart.Api.DTOs;

public class TrackEventDto
{
    public string EventType { get; set; } = string.Empty;
    public int? ProductId { get; set; }
    public int? Quantity { get; set; }
}
