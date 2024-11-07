using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateDTOValidator()
    {
        RuleFor(x => x.FirstName).NotEqual("string").WithMessage("Invalid value for FirstName.");
        RuleFor(x => x.LastName).NotEqual("string").WithMessage("Invalid value for LastName.");
        RuleFor(x => x.PhoneNumber).NotEqual("string").WithMessage("Invalid value for PhoneNumber.");
        RuleFor(x => x.Email).NotEqual("string").WithMessage("Invalid value for Email.");

        RuleFor(x => x.FirstName)
             .NotEmpty().WithMessage("FirstName can not be null")
             .MaximumLength(16).WithMessage("FirstName limit exceeded (max 16 characters)");
    }
}
