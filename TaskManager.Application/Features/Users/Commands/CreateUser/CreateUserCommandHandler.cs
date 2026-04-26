using MediatR;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    private readonly IUserService _userService;

    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        return _userService.CreateAsync(request.Request, cancellationToken);
    }
}
