using MediatR;
using TaskManager.Application.DTOs.Users;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyCollection<UserResponseDto>>
{
    private readonly IUserService _userService;

    public GetAllUsersQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public Task<IReadOnlyCollection<UserResponseDto>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        return _userService.GetAllAsync(cancellationToken);
    }
}
