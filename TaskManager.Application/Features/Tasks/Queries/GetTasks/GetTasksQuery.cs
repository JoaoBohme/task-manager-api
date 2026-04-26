using MediatR;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Features.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(TaskListQueryDto Request) : IRequest<PagedResult<TaskResponseDto>>;
