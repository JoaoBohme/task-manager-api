using MediatR;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponseDto>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<UserResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return _userService.GetByIdAsync(request.UserId, cancellationToken);
    }
}
