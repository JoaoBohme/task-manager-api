using FluentValidation;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(login => login.Email)
            .NotEmpty()
            .MaximumLength(200)
            .EmailAddress();

        RuleFor(login => login.Password)
            .NotEmpty()
            .MaximumLength(100);
    }
}
