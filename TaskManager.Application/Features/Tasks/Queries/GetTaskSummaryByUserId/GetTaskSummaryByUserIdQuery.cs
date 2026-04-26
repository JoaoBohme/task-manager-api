using MediatR;
using TaskManager.Application.Common.Models;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskSummaryByUserId;

public sealed record GetTaskSummaryByUserIdQuery(Guid UserId) : IRequest<TaskSummaryResult>;
