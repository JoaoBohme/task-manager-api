using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }

    public async Task<PagedResult<TaskItem>> GetPagedAsync(
        TaskListQueryDto query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> tasksQuery = _context.Tasks.AsNoTracking();

        if (query.Status.HasValue)
        {
            tasksQuery = tasksQuery.Where(task => task.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            tasksQuery = tasksQuery.Where(task => task.Priority == query.Priority.Value);
        }

        if (query.UserId.HasValue)
        {
            tasksQuery = tasksQuery.Where(task => task.UserId == query.UserId.Value);
        }

        var totalCount = await tasksQuery.CountAsync(cancellationToken);

        var items = await tasksQuery
            .OrderByDescending(task => task.DateCreated)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TaskItem>
        {
            Items = items,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyCollection<TaskSummaryItem>> GetSummaryByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .AsNoTracking()
            .Where(task => task.UserId == userId)
            .GroupBy(task => task.Status)
            .Select(group => new TaskSummaryItem
            {
                Status = group.Key,
                Total = group.Count()
            })
            .OrderBy(item => item.Status)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        await _context.Tasks.AddAsync(taskItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Update(taskItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Remove(taskItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
