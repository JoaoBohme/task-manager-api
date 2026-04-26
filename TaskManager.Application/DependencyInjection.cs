using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Common.Caching;
using TaskManager.Application.Interfaces.Services;
using TaskManager.Application.Services;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddSingleton<TaskListCacheState>();
        services.AddSingleton<UserListCacheState>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
