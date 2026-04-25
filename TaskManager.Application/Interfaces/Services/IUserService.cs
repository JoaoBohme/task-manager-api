using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Interfaces.Services;

public interface IUserService
{
    Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserResponseDto> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateAsync(Guid id, UpdateUserDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
