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
    public async Task GetAllAsync_ShouldReturnUsersAndReuseCache()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        userRepository.Seed(
            new User("Carlos", "carlos@taskmanager.com", "hash-1"),
            new User("Ana", "ana@taskmanager.com", "hash-2"));

        var service = CreateService(userRepository, passwordHasher);

        var firstResult = await service.GetAllAsync();
        var secondResult = await service.GetAllAsync();

        Assert.Equal(2, firstResult.Count);
        Assert.Equal(1, userRepository.GetAllCalls);
        Assert.Equal(firstResult.Select(user => user.Id), secondResult.Select(user => user.Id));
    }

    [Fact]
    public async Task GetAllAsync_ShouldInvalidateCache_AfterUserCreation()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        userRepository.Seed(new User("Carlos", "carlos@taskmanager.com", "hash-1"));

        var service = CreateService(userRepository, passwordHasher);

        var beforeCreate = await service.GetAllAsync();
        await service.GetAllAsync();

        await service.CreateAsync(new CreateUserDto
        {
            Name = "Ana",
            Email = "ana@taskmanager.com",
            Password = "SenhaForte123"
        });

        var afterCreate = await service.GetAllAsync();

        Assert.Single(beforeCreate);
        Assert.Equal(2, afterCreate.Count);
        Assert.Equal(2, userRepository.GetAllCalls);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var user = new User("Carlos", "carlos@taskmanager.com", "hash-1");
        userRepository.Seed(user);

        var service = CreateService(userRepository, passwordHasher);

        var result = await service.GetByIdAsync(user.Id);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = CreateService(new InMemoryUserRepository(), new FakePasswordHasher());

        var action = async () => await service.GetByIdAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

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
        Assert.Equal(1, userRepository.AddCalls);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        userRepository.Seed(new User("Carlos", "carlos@taskmanager.com", "hash-1"));

        var service = CreateService(userRepository, passwordHasher);

        var action = async () => await service.CreateAsync(new CreateUserDto
        {
            Name = "Outro Carlos",
            Email = "carlos@taskmanager.com",
            Password = "SenhaForte123"
        });

        await Assert.ThrowsAsync<ConflictException>(action);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenDataIsValid()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var user = new User("Carlos", "carlos@taskmanager.com", "hash-1");
        userRepository.Seed(user);

        var service = CreateService(userRepository, passwordHasher);

        var result = await service.UpdateAsync(user.Id, new UpdateUserDto
        {
            Name = "Carlos Silva",
            Email = "carlos.silva@taskmanager.com"
        });

        Assert.Equal(user.Id, result.Id);
        Assert.Equal("Carlos Silva", result.Name);
        Assert.Equal("carlos.silva@taskmanager.com", result.Email);
        Assert.Equal(1, userRepository.UpdateCalls);
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

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = CreateService(new InMemoryUserRepository(), new FakePasswordHasher());

        var action = async () => await service.UpdateAsync(
            Guid.NewGuid(),
            new UpdateUserDto
            {
                Name = "Nome",
                Email = "nome@taskmanager.com"
            });

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser_WhenUserExists()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var user = new User("Carlos", "carlos@taskmanager.com", "hash-1");
        userRepository.Seed(user);

        var service = CreateService(userRepository, passwordHasher);

        await service.DeleteAsync(user.Id);

        Assert.Equal(1, userRepository.DeleteCalls);

        var action = async () => await service.GetByIdAsync(user.Id);
        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = CreateService(new InMemoryUserRepository(), new FakePasswordHasher());

        var action = async () => await service.DeleteAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(action);
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
