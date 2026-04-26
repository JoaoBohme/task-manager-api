namespace TaskManager.Application.Common.Caching;

public class UserListCacheOptions
{
    public const string SectionName = "UserListCache";

    public int AbsoluteExpirationMinutes { get; init; } = 2;
}
