using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Common.Models;

public class TaskSummaryItem
{
    public DomainTaskStatus Status { get; init; }
    public int Total { get; init; }
}
