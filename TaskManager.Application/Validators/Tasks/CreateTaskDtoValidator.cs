using FluentValidation;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validators.Tasks;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(task => task.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(task => task.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(task => task.Priority)
            .IsInEnum();

        RuleFor(task => task.UserId)
            .NotEmpty();
    }
}
