using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Caching;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Application.Interfaces.Security;
using TaskManager.Application.Interfaces.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<UserService> _logger;
    private readonly UserListCacheState _userListCacheState;
    private readonly UserListCacheOptions _userListCacheOptions;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMemoryCache memoryCache,
        ILogger<UserService> logger,
        UserListCacheState userListCacheState,
        IOptions<UserListCacheOptions> userListCacheOptions,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<UpdateUserDto> updateUserValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _memoryCache = memoryCache;
        _logger = logger;
        _userListCacheState = userListCacheState;
        _userListCacheOptions = userListCacheOptions.Value;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    public async Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildUserListCacheKey();

        if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyCollection<UserResponseDto>? cachedUsers) &&
            cachedUsers is not null)
        {
            _logger.LogInformation(
                "Cache hit para listagem de usuarios. Chave: {CacheKey}",
                cacheKey);
            return cachedUsers;
        }

        _logger.LogInformation(
            "Cache miss para listagem de usuarios. Chave: {CacheKey}",
            cacheKey);

        var users = await _userRepository.GetAllAsync(cancellationToken);
        var result = users.Select(MapToResponse).ToList();

        _memoryCache.Set(
            cacheKey,
            result,
            TimeSpan.FromMinutes(_userListCacheOptions.AbsoluteExpirationMinutes));

        _logger.LogInformation(
            "Resultado da listagem de usuarios armazenado em cache. Chave: {CacheKey}",
            cacheKey);

        return result;
    }

    public async Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Usuario '{id}' nao encontrado.");

        return MapToResponse(user);
    }

    public async Task<UserResponseDto> CreateAsync(
        CreateUserDto request,
        CancellationToken cancellationToken = default)
    {
        await _createUserValidator.ValidateAndThrowAsync(request, cancellationToken);

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken: cancellationToken))
        {
            throw new ConflictException("Ja existe um usuario cadastrado com este email.");
        }

        var user = new User(
            request.Name,
            request.Email,
            _passwordHasher.Hash(request.Password));

        await _userRepository.AddAsync(user, cancellationToken);
        InvalidateUserListCache();

        return MapToResponse(user);
    }

    public async Task<UserResponseDto> UpdateAsync(
        Guid id,
        UpdateUserDto request,
        CancellationToken cancellationToken = default)
    {
        await _updateUserValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Usuario '{id}' nao encontrado.");

        if (await _userRepository.EmailExistsAsync(request.Email, id, cancellationToken))
        {
            throw new ConflictException("Ja existe um usuario cadastrado com este email.");
        }

        user.UpdateProfile(request.Name, request.Email);

        await _userRepository.UpdateAsync(user, cancellationToken);
        InvalidateUserListCache();

        return MapToResponse(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Usuario '{id}' nao encontrado.");

        await _userRepository.DeleteAsync(user, cancellationToken);
        InvalidateUserListCache();
    }

    private string BuildUserListCacheKey()
    {
        return string.Join(":", "users", "list", _userListCacheState.Version);
    }

    private void InvalidateUserListCache()
    {
        var newVersion = _userListCacheState.IncrementVersion();

        _logger.LogInformation(
            "Cache da listagem de usuarios invalidado. Nova versao: {CacheVersion}",
            newVersion);
    }

    private static UserResponseDto MapToResponse(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
