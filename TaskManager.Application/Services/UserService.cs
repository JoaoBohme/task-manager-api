using FluentValidation;
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
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<UpdateUserDto> updateUserValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
    }

    public async Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToResponse).ToList();
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

        return MapToResponse(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Usuario '{id}' nao encontrado.");

        await _userRepository.DeleteAsync(user, cancellationToken);
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
