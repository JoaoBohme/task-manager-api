using MediatR;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskResponseDto>
{
    private readonly ITaskService _taskService;

    public CreateTaskCommandHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<TaskResponseDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        return _taskService.CreateAsync(request.Request, cancellationToken);
    }
}
