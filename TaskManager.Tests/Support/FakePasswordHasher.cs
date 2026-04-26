using TaskManager.Application.Interfaces.Security;

namespace TaskManager.Tests.Support;

internal class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return $"hashed::{password}";
    }

    public bool Verify(string password, string passwordHash)
    {
        return passwordHash == Hash(password);
    }
}
