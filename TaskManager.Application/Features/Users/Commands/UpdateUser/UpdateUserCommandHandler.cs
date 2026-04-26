using MediatR;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return _userService.UpdateAsync(request.UserId, request.Request, cancellationToken);
    }
}
