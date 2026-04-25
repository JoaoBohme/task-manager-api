using TaskManager.Application.Common.Models;
using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Application.DTOs.Tasks;

public class TaskListQueryDto : PaginationParameters
{
    public DomainTaskStatus? Status { get; init; }
    public TaskPriority? Priority { get; init; }
    public Guid? UserId { get; init; }
}
