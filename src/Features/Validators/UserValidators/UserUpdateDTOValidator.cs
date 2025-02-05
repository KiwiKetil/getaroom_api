using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateDTOValidator()
    {
        RuleFor(x => x.FirstName)
             .NotEmpty().WithMessage("FirstName can not be empty")
             .NotEqual("string").WithMessage("Invalid value for Firstname.")
             .MaximumLength(16).WithMessage("FirstName limit exceeded (max 16 characters)");

        RuleFor(x => x.LastName)
             .NotEmpty().WithMessage("LastName can not be empty")
             .NotEqual("string").WithMessage("Invalid value for Lastname.")
             .MaximumLength(24).WithMessage("Lastname limit exceeded (max 24 characters)");

        RuleFor(x => x.PhoneNumber)
           .NotEmpty().WithMessage("Phonenumber can not be null")
           .Length(8).WithMessage("Phonenumber must be 8 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email must be included")
            .EmailAddress().WithMessage("Email must be a valid email address");
    }
}
