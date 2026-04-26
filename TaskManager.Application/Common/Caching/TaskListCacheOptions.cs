namespace TaskManager.Application.Common.Caching;

public class TaskListCacheOptions
{
    public const string SectionName = "TaskListCache";

    public int AbsoluteExpirationMinutes { get; init; } = 2;
}
