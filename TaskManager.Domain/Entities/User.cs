namespace TaskManager.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public ICollection<TaskItem> Tasks { get; private set; }

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
        Tasks = new List<TaskItem>();
    }

    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = NormalizeRequiredText(name);
        Email = NormalizeEmail(email);
        PasswordHash = NormalizeRequiredText(passwordHash);
        Tasks = new List<TaskItem>();
    }

    public void UpdateProfile(string name, string email)
    {
        Name = NormalizeRequiredText(name);
        Email = NormalizeEmail(email);
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = NormalizeRequiredText(passwordHash);
    }

    public void AddTask(TaskItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        Tasks.Add(task);
    }

    private static string NormalizeRequiredText(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Value cannot be empty.", nameof(value))
            : value.Trim();
    }

    private static string NormalizeEmail(string email)
    {
        var normalizedEmail = NormalizeRequiredText(email);
        return normalizedEmail.ToLowerInvariant();
    }
}
