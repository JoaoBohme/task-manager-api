using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class TaskListQueryDtoValidator : AbstractValidator<TaskListQueryDto>
{
    public TaskListQueryDtoValidator()
    {
        RuleFor(task => task.PageNumber)
            .GreaterThan(0);

        RuleFor(task => task.PageSize)
            .InclusiveBetween(1, 100);

        When(task => task.Status.HasValue, () =>
        {
            RuleFor(task => task.Status!.Value)
                .IsInEnum();
        });

        When(task => task.Priority.HasValue, () =>
        {
            RuleFor(task => task.Priority!.Value)
                .IsInEnum();
        });
    }
}
