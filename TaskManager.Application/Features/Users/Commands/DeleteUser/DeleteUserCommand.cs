using MediatR;

namespace TaskManager.Application.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest;
