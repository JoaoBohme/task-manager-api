using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Application.DTOs.Tasks;

public class TaskResponseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DomainTaskStatus Status { get; init; }
    public TaskPriority Priority { get; init; }
    public DateTime DateCreated { get; init; }
    public DateTime? DateCompleted { get; init; }
    public Guid UserId { get; init; }
}
