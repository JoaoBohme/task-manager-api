namespace TaskManager.Application.Common.Models;

public class TaskSummaryResult
{
    public Guid UserId { get; init; }
    public IReadOnlyCollection<TaskSummaryItem> Items { get; init; } = [];
}
