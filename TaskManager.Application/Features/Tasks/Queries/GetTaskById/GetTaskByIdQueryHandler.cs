using MediatR;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskResponseDto>
{
    private readonly ITaskService _taskService;

    public GetTaskByIdQueryHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<TaskResponseDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        return _taskService.GetByIdAsync(request.TaskId, cancellationToken);
    }
}
