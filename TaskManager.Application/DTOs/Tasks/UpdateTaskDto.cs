using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Application.DTOs.Tasks;

public class UpdateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DomainTaskStatus Status { get; init; }
    public TaskPriority Priority { get; init; }
}
