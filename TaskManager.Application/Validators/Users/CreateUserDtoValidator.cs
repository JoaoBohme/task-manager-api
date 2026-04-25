using FluentValidation;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Validators.Users;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(user => user.Email)
            .NotEmpty()
            .MaximumLength(200)
            .EmailAddress();

        RuleFor(user => user.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100);
    }
}
