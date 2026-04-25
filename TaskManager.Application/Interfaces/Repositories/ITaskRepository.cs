using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskItem>> GetPagedAsync(
        TaskListQueryDto query,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TaskSummaryItem>> GetSummaryByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(TaskItem taskItem, CancellationToken cancellationToken = default);
}
