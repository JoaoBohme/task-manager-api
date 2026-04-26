using MediatR;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginDto Request) : IRequest<AuthResponseDto>;
