using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Caching;
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
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TaskService> _logger;
    private readonly TaskListCacheState _taskListCacheState;
    private readonly TaskListCacheOptions _taskListCacheOptions;
    private readonly IValidator<CreateTaskDto> _createTaskValidator;
    private readonly IValidator<UpdateTaskDto> _updateTaskValidator;
    private readonly IValidator<TaskListQueryDto> _taskListQueryValidator;

    public TaskService(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IMemoryCache memoryCache,
        ILogger<TaskService> logger,
        TaskListCacheState taskListCacheState,
        IOptions<TaskListCacheOptions> taskListCacheOptions,
        IValidator<CreateTaskDto> createTaskValidator,
        IValidator<UpdateTaskDto> updateTaskValidator,
        IValidator<TaskListQueryDto> taskListQueryValidator)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _memoryCache = memoryCache;
        _logger = logger;
        _taskListCacheState = taskListCacheState;
        _taskListCacheOptions = taskListCacheOptions.Value;
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

        var cacheKey = BuildTaskListCacheKey(query);

        if (_memoryCache.TryGetValue(cacheKey, out PagedResult<TaskResponseDto>? cachedResult) &&
            cachedResult is not null)
        {
            _logger.LogInformation(
                "Cache hit para listagem de tarefas. Chave: {CacheKey}",
                cacheKey);
            return cachedResult;
        }

        _logger.LogInformation(
            "Cache miss para listagem de tarefas. Chave: {CacheKey}",
            cacheKey);

        var pagedTasks = await _taskRepository.GetPagedAsync(query, cancellationToken);

        var result = new PagedResult<TaskResponseDto>
        {
            Items = pagedTasks.Items.Select(MapToResponse).ToList(),
            PageNumber = pagedTasks.PageNumber,
            PageSize = pagedTasks.PageSize,
            TotalCount = pagedTasks.TotalCount
        };

        _memoryCache.Set(
            cacheKey,
            result,
            TimeSpan.FromMinutes(_taskListCacheOptions.AbsoluteExpirationMinutes));

        _logger.LogInformation(
            "Resultado da listagem de tarefas armazenado em cache. Chave: {CacheKey}",
            cacheKey);

        return result;
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
        InvalidateTaskListCache();

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
        InvalidateTaskListCache();

        return MapToResponse(taskItem);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskItem = await _taskRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Tarefa '{id}' nao encontrada.");

        await _taskRepository.DeleteAsync(taskItem, cancellationToken);
        InvalidateTaskListCache();
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

    private string BuildTaskListCacheKey(TaskListQueryDto query)
    {
        return string.Join(
            ":",
            "tasks",
            "list",
            _taskListCacheState.Version,
            query.PageNumber,
            query.PageSize,
            query.Status?.ToString() ?? "null",
            query.Priority?.ToString() ?? "null",
            query.UserId?.ToString() ?? "null");
    }

    private void InvalidateTaskListCache()
    {
        var newVersion = _taskListCacheState.IncrementVersion();

        _logger.LogInformation(
            "Cache da listagem de tarefas invalidado. Nova versao: {CacheVersion}",
            newVersion);
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
