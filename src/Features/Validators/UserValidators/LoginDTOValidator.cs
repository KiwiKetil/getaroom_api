using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    private const string PasswordPattern = @"^(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[!?*#_-]).{8,24}$";

    public LoginDTOValidator()
    {
        RuleFor(x => x.Email)                 
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(PasswordPattern).WithMessage("Password must be 8-24 characters, include at least 1 number, 1 uppercase, 1 lowercase, and 1 special character ('! ? * # _ -')");
    }
}
