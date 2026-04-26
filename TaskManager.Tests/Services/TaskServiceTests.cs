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
        Assert.NotNull(result.DateCreated);
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
    }

    private static TaskService CreateService(
        InMemoryTaskRepository taskRepository,
        InMemoryUserRepository userRepository)
    {
        return new TaskService(
            taskRepository,
            userRepository,
            new CreateTaskDtoValidator(),
            new UpdateTaskDtoValidator(),
            new TaskListQueryDtoValidator());
    }
}
