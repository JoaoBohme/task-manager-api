using MediatR;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, UpdateUserDto Request) : IRequest<UserResponseDto>;
