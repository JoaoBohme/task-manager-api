using System.Threading;

namespace TaskManager.Application.Common.Caching;

public class TaskListCacheState
{
    private long _version;

    public long Version => Interlocked.Read(ref _version);

    public long IncrementVersion()
    {
        return Interlocked.Increment(ref _version);
    }
}
