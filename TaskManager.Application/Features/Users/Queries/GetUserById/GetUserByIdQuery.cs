using MediatR;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Features.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserResponseDto>;
