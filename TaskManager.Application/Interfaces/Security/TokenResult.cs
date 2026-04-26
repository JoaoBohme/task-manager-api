namespace TaskManager.Application.Interfaces.Security;

public class TokenResult
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
