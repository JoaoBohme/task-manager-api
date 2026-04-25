namespace TaskManager.Domain.Entities;

public class User
{
    private readonly List<TaskItem> _tasks = [];

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string name, string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Name = NormalizeRequiredText(name);
        Email = NormalizeEmail(email);
        PasswordHash = NormalizeRequiredText(passwordHash);
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
        _tasks.Add(task);
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
