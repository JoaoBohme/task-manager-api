using MediatR;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(Guid TaskId, UpdateTaskDto Request) : IRequest<TaskResponseDto>;
