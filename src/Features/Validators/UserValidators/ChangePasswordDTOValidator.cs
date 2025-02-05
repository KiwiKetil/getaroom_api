using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class ChangePasswordDTOValidator : AbstractValidator<ChangePasswordDTO>
{
    private const string PasswordPattern = @"^(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[!?*#_-]).{8,24}$";

    public ChangePasswordDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.CurrentPassword)
           .NotEmpty().WithMessage("CurrentPassword cannot be empty")
           .Matches(PasswordPattern).WithMessage("Password must be 8-24 characters, include at least 1 number, 1 uppercase, 1 lowercase, and 1 special character ('! ? * # _ -')");

        RuleFor(x => x.NewPassword)
           .NotEmpty().WithMessage("NewPassword cannot be empty")
           .Matches(PasswordPattern).WithMessage("Password must be 8-24 characters, include at least 1 number, 1 uppercase, 1 lowercase, and 1 special character ('! ? * # _ -')");
    }
}
