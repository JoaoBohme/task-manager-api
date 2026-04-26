using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Caching;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Services;
using TaskManager.Application.Validators.Users;
using TaskManager.Domain.Entities;
using TaskManager.Tests.Support;

namespace TaskManager.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldHashPasswordAndPersistUser()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var service = CreateService(userRepository, passwordHasher);

        var request = new CreateUserDto
        {
            Name = "Carlos",
            Email = "carlos@taskmanager.com",
            Password = "SenhaForte123"
        };

        var result = await service.CreateAsync(request);
        var persistedUser = await userRepository.GetByIdAsync(result.Id);

        Assert.NotNull(persistedUser);
        Assert.Equal("hashed::SenhaForte123", persistedUser!.PasswordHash);
        Assert.Equal("carlos@taskmanager.com", persistedUser.Email);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var existingUser = new User("Carlos", "carlos@taskmanager.com", "hash-1");
        var anotherUser = new User("Maria", "maria@taskmanager.com", "hash-2");
        userRepository.Seed(existingUser, anotherUser);

        var service = CreateService(userRepository, passwordHasher);

        var request = new UpdateUserDto
        {
            Name = "Maria",
            Email = "carlos@taskmanager.com"
        };

        var action = async () => await service.UpdateAsync(anotherUser.Id, request);

        var exception = await Assert.ThrowsAsync<ConflictException>(action);
        Assert.Contains("email", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static UserService CreateService(
        InMemoryUserRepository userRepository,
        FakePasswordHasher passwordHasher)
    {
        return new UserService(
            userRepository,
            passwordHasher,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<UserService>.Instance,
            new UserListCacheState(),
            Options.Create(new UserListCacheOptions()),
            new CreateUserDtoValidator(),
            new UpdateUserDtoValidator());
    }
}
