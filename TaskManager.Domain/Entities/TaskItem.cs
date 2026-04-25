using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DomainTaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime DateCreated { get; private set; }
    public DateTime? DateCompleted { get; private set; }
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    private TaskItem()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    public TaskItem(
        string title,
        string description,
        TaskPriority priority,
        Guid userId)
    {
        Id = Guid.NewGuid();
        Title = NormalizeRequiredText(title);
        Description = NormalizeRequiredText(description);
        Status = DomainTaskStatus.Pending;
        Priority = priority;
        DateCreated = DateTime.UtcNow;
        UserId = userId;
    }

    public void UpdateDetails(string title, string description, TaskPriority priority)
    {
        Title = NormalizeRequiredText(title);
        Description = NormalizeRequiredText(description);
        Priority = priority;
    }

    public void ChangeStatus(DomainTaskStatus status)
    {
        Status = status;
        DateCompleted = status == DomainTaskStatus.Completed
            ? DateTime.UtcNow
            : null;
    }

    private static string NormalizeRequiredText(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be empty.", nameof(value))
            : value.Trim();
    }
}
