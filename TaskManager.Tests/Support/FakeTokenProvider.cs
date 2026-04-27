using TaskManager.Application.Interfaces.Security;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Support;

internal class FakeTokenProvider : ITokenProvider
{
    public int GenerateTokenCalls { get; private set; }
    public User? LastUser { get; private set; }

    public TokenResult GenerateToken(User user)
    {
        GenerateTokenCalls++;
        LastUser = user;

        return new TokenResult
        {
            AccessToken = $"token-for::{user.Email}",
            ExpiresAtUtc = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
    }
}
