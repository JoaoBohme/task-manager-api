using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Security;

public interface ITokenProvider
{
    TokenResult GenerateToken(User user);
}
