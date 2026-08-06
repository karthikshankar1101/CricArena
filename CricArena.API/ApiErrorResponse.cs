namespace CricArena.API.Models;

public class ApiErrorResponse
{
    public bool Success { get; set; } = false;

    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? TraceId { get; set; }
}