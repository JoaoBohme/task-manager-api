using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Support;

internal class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = [];

    public int GetPagedCalls { get; private set; }
    public int GetByIdCalls { get; private set; }
    public int GetSummaryCalls { get; private set; }
    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int DeleteCalls { get; private set; }

    public Task<IReadOnlyCollection<TaskSummaryItem>> GetSummaryByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        GetSummaryCalls++;
        IReadOnlyCollection<TaskSummaryItem> summary = _tasks
            .Where(task => task.UserId == userId)
            .GroupBy(task => task.Status)
            .Select(group => new TaskSummaryItem
            {
                Status = group.Key,
                Total = group.Count()
            })
            .OrderBy(item => item.Status)
            .ToList();

        return Task.FromResult(summary);
    }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        GetByIdCalls++;
        return Task.FromResult(_tasks.FirstOrDefault(task => task.Id == id));
    }

    public Task<PagedResult<TaskItem>> GetPagedAsync(
        TaskListQueryDto query,
        CancellationToken cancellationToken = default)
    {
        GetPagedCalls++;
        var filteredTasks = _tasks.AsEnumerable();

        if (query.Status.HasValue)
        {
            filteredTasks = filteredTasks.Where(task => task.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            filteredTasks = filteredTasks.Where(task => task.Priority == query.Priority.Value);
        }

        if (query.UserId.HasValue)
        {
            filteredTasks = filteredTasks.Where(task => task.UserId == query.UserId.Value);
        }

        var orderedTasks = filteredTasks
            .OrderByDescending(task => task.DateCreated)
            .ToList();

        var pagedTasks = orderedTasks
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        return Task.FromResult(new PagedResult<TaskItem>
        {
            Items = pagedTasks,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = orderedTasks.Count
        });
    }

    public Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        AddCalls++;
        _tasks.Add(taskItem);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        DeleteCalls++;
        _tasks.Remove(taskItem);
        return Task.CompletedTask;
    }

    public void Seed(params TaskItem[] tasks)
    {
        _tasks.AddRange(tasks);
    }
}
