using MediatR;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<IReadOnlyCollection<UserResponseDto>>;
