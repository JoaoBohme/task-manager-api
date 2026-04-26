using MediatR;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(CreateUserDto Request) : IRequest<UserResponseDto>;
