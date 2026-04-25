using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(task => task.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(task => task.Status)
            .IsInEnum();

        RuleFor(task => task.Priority)
            .IsInEnum();
    }
}
