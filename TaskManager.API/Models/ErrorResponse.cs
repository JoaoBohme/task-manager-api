namespace TaskManager.API.Models;

public class ErrorResponse
{
    public string TraceId { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public object? Details { get; init; }
    public int StatusCode { get; init; }
}
