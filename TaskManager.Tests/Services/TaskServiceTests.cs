using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TaskManager.Application.Common.Caching;
using TaskManager.Application.Common.Exceptions;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Services;
using TaskManager.Application.Validators.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Tests.Support;
using DomainTaskStatus = TaskManager.Domain.Enums.TaskStatus;
using TaskPriority = TaskManager.Domain.Enums.TaskPriority;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreatePendingTask_WhenUserExists()
    {
        var userRepository = new InMemoryUserRepository();
        var taskRepository = new InMemoryTaskRepository();
        var user = new User("Joao", "joao@taskmanager.com", "hashed-password");
        userRepository.Seed(user);
        var service = CreateService(taskRepository, userRepository);

        var request = new CreateTaskDto
        {
            Title = "Preparar entrega",
            Description = "Finalizar o teste tecnico",
            Priority = TaskPriority.High,
            UserId = user.Id
        };

        var result = await service.CreateAsync(request);

        Assert.Equal(request.Title, result.Title);
        Assert.Equal(DomainTaskStatus.Pending, result.Status);
        Assert.Equal(request.UserId, result.UserId);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default, result.DateCreated);
        Assert.Equal(1, taskRepository.AddCalls);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = CreateService(new InMemoryTaskRepository(), new InMemoryUserRepository());
        var request = new CreateTaskDto
        {
            Title = "Tarefa invalida",
            Description = "Usuario inexistente",
            Priority = TaskPriority.Medium,
            UserId = Guid.NewGuid()
        };

        var action = async () => await service.CreateAsync(request);

        var exception = await Assert.ThrowsAsync<NotFoundException>(action);
        Assert.Contains("Usuario", exception.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        var user = new User("Maria", "maria@taskmanager.com", "hash");
        var task = new TaskItem("Planejar deploy", "Revisar checklist", TaskPriority.Medium, user.Id);
        var taskRepository = new InMemoryTaskRepository();
        var userRepository = new InMemoryUserRepository();

        taskRepository.Seed(task);
        userRepository.Seed(user);

        var service = CreateService(taskRepository, userRepository);

        var result = await service.GetByIdAsync(task.Id);

        Assert.Equal(task.Id, result.Id);
        Assert.Equal(task.Title, result.Title);
        Assert.Equal(task.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        var service = CreateService(new InMemoryTaskRepository(), new InMemoryUserRepository());

        var action = async () => await service.GetByIdAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldApplyFiltersAndReuseCache_ForSameQuery()
    {
        var userA = new User("Ana", "ana@taskmanager.com", "hash-a");
        var userB = new User("Bruno", "bruno@taskmanager.com", "hash-b");
        var task1 = new TaskItem("T1", "D1", TaskPriority.High, userA.Id);
        var task2 = new TaskItem("T2", "D2", TaskPriority.High, userA.Id);
        var task3 = new TaskItem("T3", "D3", TaskPriority.Low, userB.Id);
        task2.ChangeStatus(DomainTaskStatus.InProgress);
        task3.ChangeStatus(DomainTaskStatus.Completed);

        var taskRepository = new InMemoryTaskRepository();
        var userRepository = new InMemoryUserRepository();
        taskRepository.Seed(task1, task2, task3);
        userRepository.Seed(userA, userB);

        var service = CreateService(taskRepository, userRepository);
        var query = new TaskListQueryDto
        {
            PageNumber = 1,
            PageSize = 10,
            Status = DomainTaskStatus.InProgress,
            Priority = TaskPriority.High,
            UserId = userA.Id
        };

        var firstResult = await service.GetPagedAsync(query);
        var secondResult = await service.GetPagedAsync(query);

        var item = Assert.Single(firstResult.Items);
        Assert.Equal(task2.Id, item.Id);
        Assert.Equal(1, taskRepository.GetPagedCalls);
        Assert.Single(secondResult.Items);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldInvalidateCache_AfterTaskCreation()
    {
        var user = new User("Ana", "ana@taskmanager.com", "hash");
        var taskRepository = new InMemoryTaskRepository();
        var userRepository = new InMemoryUserRepository();
        userRepository.Seed(user);

        var existingTask = new TaskItem("T1", "D1", TaskPriority.Low, user.Id);
        taskRepository.Seed(existingTask);

        var service = CreateService(taskRepository, userRepository);
        var query = new TaskListQueryDto { PageNumber = 1, PageSize = 10, UserId = user.Id };

        var beforeCreate = await service.GetPagedAsync(query);
        await service.GetPagedAsync(query);

        await service.CreateAsync(new CreateTaskDto
        {
            Title = "Nova tarefa",
            Description = "Descricao",
            Priority = TaskPriority.High,
            UserId = user.Id
        });

        var afterCreate = await service.GetPagedAsync(query);

        Assert.Equal(2, taskRepository.GetPagedCalls);
        Assert.Single(beforeCreate.Items);
        Assert.Equal(2, afterCreate.Items.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetDateCompleted_WhenStatusChangesToCompleted()
    {
        var userRepository = new InMemoryUserRepository();
        var taskRepository = new InMemoryTaskRepository();
        var user = new User("Maria", "maria@taskmanager.com", "hashed-password");
        var task = new TaskItem("Planejar deploy", "Revisar checklist", TaskPriority.Medium, user.Id);

        userRepository.Seed(user);
        taskRepository.Seed(task);

        var service = CreateService(taskRepository, userRepository);

        var request = new UpdateTaskDto
        {
            Title = "Planejar deploy final",
            Description = "Checklist validado",
            Priority = TaskPriority.High,
            Status = DomainTaskStatus.Completed
        };

        var result = await service.UpdateAsync(task.Id, request);

        Assert.Equal(DomainTaskStatus.Completed, result.Status);
        Assert.NotNull(result.DateCompleted);
        Assert.Equal("Planejar deploy final", result.Title);
        Assert.Equal(1, taskRepository.UpdateCalls);
    }

    [Fact]
    public async Task UpdateAsync_ShouldClearDateCompleted_WhenStatusChangesFromCompleted()
    {
        var userRepository = new InMemoryUserRepository();
        var taskRepository = new InMemoryTaskRepository();
        var user = new User("Maria", "maria@taskmanager.com", "hashed-password");
        var task = new TaskItem("Planejar deploy", "Revisar checklist", TaskPriority.Medium, user.Id);
        task.ChangeStatus(DomainTaskStatus.Completed);

        userRepository.Seed(user);
        taskRepository.Seed(task);

        var service = CreateService(taskRepository, userRepository);

        var request = new UpdateTaskDto
        {
            Title = "Planejar deploy",
            Description = "Revisar checklist novamente",
            Priority = TaskPriority.Medium,
            Status = DomainTaskStatus.InProgress
        };

        var result = await service.UpdateAsync(task.Id, request);

        Assert.Equal(DomainTaskStatus.InProgress, result.Status);
        Assert.Null(result.DateCompleted);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        var service = CreateService(new InMemoryTaskRepository(), new InMemoryUserRepository());

        var action = async () => await service.UpdateAsync(
            Guid.NewGuid(),
            new UpdateTaskDto
            {
                Title = "Titulo",
                Description = "Descricao",
                Priority = TaskPriority.Low,
                Status = DomainTaskStatus.Pending
            });

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask_WhenTaskExists()
    {
        var user = new User("Ana", "ana@taskmanager.com", "hash");
        var task = new TaskItem("T1", "D1", TaskPriority.Low, user.Id);
        var taskRepository = new InMemoryTaskRepository();
        var userRepository = new InMemoryUserRepository();
        taskRepository.Seed(task);
        userRepository.Seed(user);

        var service = CreateService(taskRepository, userRepository);

        await service.DeleteAsync(task.Id);

        Assert.Equal(1, taskRepository.DeleteCalls);

        var action = async () => await service.GetByIdAsync(task.Id);
        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        var service = CreateService(new InMemoryTaskRepository(), new InMemoryUserRepository());

        var action = async () => await service.DeleteAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    [Fact]
    public async Task GetSummaryByUserIdAsync_ShouldReturnTotalsGroupedByStatus()
    {
        var userRepository = new InMemoryUserRepository();
        var taskRepository = new InMemoryTaskRepository();
        var user = new User("Ana", "ana@taskmanager.com", "hashed-password");
        var pendingTask = new TaskItem("Tarefa 1", "Descricao 1", TaskPriority.Low, user.Id);
        var completedTask1 = new TaskItem("Tarefa 2", "Descricao 2", TaskPriority.Medium, user.Id);
        var completedTask2 = new TaskItem("Tarefa 3", "Descricao 3", TaskPriority.High, user.Id);

        completedTask1.ChangeStatus(DomainTaskStatus.Completed);
        completedTask2.ChangeStatus(DomainTaskStatus.Completed);

        userRepository.Seed(user);
        taskRepository.Seed(pendingTask, completedTask1, completedTask2);

        var service = CreateService(taskRepository, userRepository);

        var result = await service.GetSummaryByUserIdAsync(user.Id);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, item => item.Status == DomainTaskStatus.Pending && item.Total == 1);
        Assert.Contains(result.Items, item => item.Status == DomainTaskStatus.Completed && item.Total == 2);
        Assert.Equal(1, taskRepository.GetSummaryCalls);
    }

    [Fact]
    public async Task GetSummaryByUserIdAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = CreateService(new InMemoryTaskRepository(), new InMemoryUserRepository());

        var action = async () => await service.GetSummaryByUserIdAsync(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(action);
    }

    private static TaskService CreateService(
        InMemoryTaskRepository taskRepository,
        InMemoryUserRepository userRepository)
    {
        return new TaskService(
            taskRepository,
            userRepository,
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<TaskService>.Instance,
            new TaskListCacheState(),
            Options.Create(new TaskListCacheOptions()),
            new CreateTaskDtoValidator(),
            new UpdateTaskDtoValidator(),
            new TaskListQueryDtoValidator());
    }
}
