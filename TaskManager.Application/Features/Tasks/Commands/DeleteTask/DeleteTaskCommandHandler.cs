using MediatR;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskService _taskService;

    public DeleteTaskCommandHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        await _taskService.DeleteAsync(request.TaskId, cancellationToken);
    }
}
