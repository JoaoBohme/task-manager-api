using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Queries.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedResult<TaskResponseDto>>
{
    private readonly ITaskService _taskService;

    public GetTasksQueryHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<PagedResult<TaskResponseDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        return _taskService.GetPagedAsync(request.Request, cancellationToken);
    }
}
