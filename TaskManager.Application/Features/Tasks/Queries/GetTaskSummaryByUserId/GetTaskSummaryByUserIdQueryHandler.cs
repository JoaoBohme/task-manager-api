using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Interfaces.Services;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskSummaryByUserId;

public class GetTaskSummaryByUserIdQueryHandler : IRequestHandler<GetTaskSummaryByUserIdQuery, TaskSummaryResult>
{
    private readonly ITaskService _taskService;

    public GetTaskSummaryByUserIdQueryHandler(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public Task<TaskSummaryResult> Handle(
        GetTaskSummaryByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        return _taskService.GetSummaryByUserIdAsync(request.UserId, cancellationToken);
    }
}
