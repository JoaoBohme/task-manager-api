using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Support;

internal class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public int GetAllCalls { get; private set; }
    public int GetByIdCalls { get; private set; }
    public int GetByEmailCalls { get; private set; }
    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int DeleteCalls { get; private set; }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        GetByIdCalls++;
        return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        GetByEmailCalls++;
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return Task.FromResult(_users.FirstOrDefault(user => user.Email == normalizedEmail));
    }

    public Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        GetAllCalls++;
        return Task.FromResult((IReadOnlyCollection<User>)_users.OrderBy(user => user.Name).ToList());
    }

    public Task<bool> EmailExistsAsync(
        string email,
        Guid? ignoreUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return Task.FromResult(_users.Any(user =>
            user.Email == normalizedEmail &&
            (!ignoreUserId.HasValue || user.Id != ignoreUserId.Value)));
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        AddCalls++;
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        DeleteCalls++;
        _users.Remove(user);
        return Task.CompletedTask;
    }

    public void Seed(params User[] users)
    {
        _users.AddRange(users);
    }
}
