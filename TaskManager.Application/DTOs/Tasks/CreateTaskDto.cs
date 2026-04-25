using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Tasks;

public class CreateTaskDto
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; init; }
    public Guid UserId { get; init; }
}
