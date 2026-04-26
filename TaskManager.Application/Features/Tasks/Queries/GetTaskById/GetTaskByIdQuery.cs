using MediatR;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskResponseDto>;
