using FluentValidation;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Services;
using TaskManager.Application.Validators.Auth;
using TaskManager.Domain.Entities;
using TaskManager.Tests.Support;

namespace TaskManager.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnTokenAndUser_WhenCredentialsAreValid()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var tokenProvider = new FakeTokenProvider();
        var user = new User("Admin", "admin@taskmanager.com", passwordHasher.Hash("Admin@123"));
        userRepository.Seed(user);

        var service = CreateService(userRepository, passwordHasher, tokenProvider);

        var result = await service.LoginAsync(new LoginDto
        {
            Email = "admin@taskmanager.com",
            Password = "Admin@123"
        });

        Assert.Equal($"token-for::{user.Email}", result.AccessToken);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Email, result.User.Email);
        Assert.Equal(1, tokenProvider.GenerateTokenCalls);
        Assert.Same(user, tokenProvider.LastUser);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        var userRepository = new InMemoryUserRepository();
        var passwordHasher = new FakePasswordHasher();
        var tokenProvider = new FakeTokenProvider();
        var user = new User("Admin", "admin@taskmanager.com", passwordHasher.Hash("Admin@123"));
        userRepository.Seed(user);

        var service = CreateService(userRepository, passwordHasher, tokenProvider);

        var action = async () => await service.LoginAsync(new LoginDto
        {
            Email = "admin@taskmanager.com",
            Password = "SenhaErrada"
        });

        await Assert.ThrowsAsync<UnauthorizedException>(action);
        Assert.Equal(0, tokenProvider.GenerateTokenCalls);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
    {
        var service = CreateService(
            new InMemoryUserRepository(),
            new FakePasswordHasher(),
            new FakeTokenProvider());

        var action = async () => await service.LoginAsync(new LoginDto
        {
            Email = "inexistente@taskmanager.com",
            Password = "SenhaForte123"
        });

        await Assert.ThrowsAsync<UnauthorizedException>(action);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        var service = CreateService(
            new InMemoryUserRepository(),
            new FakePasswordHasher(),
            new FakeTokenProvider());

        var action = async () => await service.LoginAsync(new LoginDto
        {
            Email = string.Empty,
            Password = string.Empty
        });

        await Assert.ThrowsAsync<ValidationException>(action);
    }

    private static AuthService CreateService(
        InMemoryUserRepository userRepository,
        FakePasswordHasher passwordHasher,
        FakeTokenProvider tokenProvider)
    {
        return new AuthService(
            userRepository,
            passwordHasher,
            tokenProvider,
            new LoginDtoValidator());
    }
}
