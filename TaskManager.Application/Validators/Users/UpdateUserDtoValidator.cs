using FluentValidation;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.Application.Validators.Users;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(user => user.Email)
            .NotEmpty()
            .MaximumLength(200)
            .EmailAddress();
    }
}
