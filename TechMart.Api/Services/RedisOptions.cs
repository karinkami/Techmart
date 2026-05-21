namespace TechMart.Api.Services;

public sealed class RedisOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = "localhost:6379";
    public string QueueKey { get; set; } = "user-events";
}
