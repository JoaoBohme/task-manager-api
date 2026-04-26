using MediatR;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return _authService.LoginAsync(request.Request, cancellationToken);
    }
}
