namespace TechMart.Api.DTOs;

public class RecommendationsRequestDto
{
    public int? UserId { get; set; }
    public int Limit { get; set; } = 5;
}
