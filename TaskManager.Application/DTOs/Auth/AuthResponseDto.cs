using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public UserResponseDto User { get; init; } = new();
}
