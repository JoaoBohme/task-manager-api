using MediatR;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskResponseDto>
{
    private readonly ITaskService _taskService;

    public UpdateTaskCommandHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<TaskResponseDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        return _taskService.UpdateAsync(request.TaskId, request.Request, cancellationToken);
    }
}
