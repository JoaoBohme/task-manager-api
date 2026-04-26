using MediatR;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(CreateTaskDto Request) : IRequest<TaskResponseDto>;
