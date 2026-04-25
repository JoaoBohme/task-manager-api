using FluentValidation;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.Common.Models;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Repositories;
using TaskManager.Application.Interfaces.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateTaskDto> _createTaskValidator;
    private readonly IValidator<UpdateTaskDto> _updateTaskValidator;
    private readonly IValidator<TaskListQueryDto> _taskListQueryValidator;

    public TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IValidator<CreateTaskDto> createTaskValidator,
        IValidator<UpdateTaskDto> updateTaskValidator,
        IValidator<TaskListQueryDto> taskListQueryValidator)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _createTaskValidator = createTaskValidator;
        _updateTaskValidator = updateTaskValidator;
        _taskListQueryValidator = taskListQueryValidator;
    }

    public async Task<TaskResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tarefa '{id}' nao encontrada.");

        return MapToResponse(taskItem);
    }

    public async Task<PagedResult<TaskResponseDto>> GetPagedAsync(
        TaskListQueryDto query,
        CancellationToken cancellationToken = default)
    {
        await _taskListQueryValidator.ValidateAndThrowAsync(query, cancellationToken);

        if (query.UserId.HasValue)
        {
            await EnsureUserExistsAsync(query.UserId.Value, cancellationToken);
        }

        var pagedTasks = await _taskRepository.GetPagedAsync(query, cancellationToken);

        return new PagedResult<TaskResponseDto>
        {
            Items = pagedTasks.Items.Select(MapToResponse).ToList(),
            PageNumber = pagedTasks.PageNumber,
            PageSize = pagedTasks.PageSize,
            TotalCount = pagedTasks.TotalCount
        };
    }

    public async Task<TaskResponseDto> CreateAsync(
        CreateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        await _createTaskValidator.ValidateAndThrowAsync(request, cancellationToken);
        await EnsureUserExistsAsync(request.UserId, cancellationToken);

        var taskItem = new TaskItem(
            request.Title,
            request.Description,
            request.Priority,
            request.UserId);

        await _taskRepository.AddAsync(taskItem, cancellationToken);

        return MapToResponse(taskItem);
    }

    public async Task<TaskResponseDto> UpdateAsync(
        Guid id,
        UpdateTaskDto request,
        CancellationToken cancellationToken = default)
    {
        await _updateTaskValidator.ValidateAndThrowAsync(request, cancellationToken);

        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tarefa '{id}' nao encontrada.");

        taskItem.UpdateDetails(request.Title, request.Description, request.Priority);
        taskItem.ChangeStatus(request.Status);

        await _taskRepository.UpdateAsync(taskItem, cancellationToken);

        return MapToResponse(taskItem);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tarefa '{id}' nao encontrada.");

        await _taskRepository.DeleteAsync(taskItem, cancellationToken);
    }

    public async Task<TaskSummaryResult> GetSummaryByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await EnsureUserExistsAsync(userId, cancellationToken);

        var summaryItems = await _taskRepository.GetSummaryByUserIdAsync(userId, cancellationToken);

        return new TaskSummaryResult
        {
            UserId = userId,
            Items = summaryItems
        };
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        _ = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"Usuario '{userId}' nao encontrado.");
    }

    private static TaskResponseDto MapToResponse(TaskItem taskItem)
    {
        return new TaskResponseDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status,
            Priority = taskItem.Priority,
            DateCreated = taskItem.DateCreated,
            DateCompleted = taskItem.DateCompleted,
            UserId = taskItem.UserId
        };
    }
}
