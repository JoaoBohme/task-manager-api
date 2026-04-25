using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Interfaces.Services;

public interface ITaskService
{
    Task<TaskResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<TaskResponseDto>> GetPagedAsync(
        TaskListQueryDto query,
        CancellationToken cancellationToken = default);
    Task<TaskResponseDto> CreateAsync(CreateTaskDto request, CancellationToken cancellationToken = default);
    Task<TaskResponseDto> UpdateAsync(Guid id, UpdateTaskDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskSummaryResult> GetSummaryByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
