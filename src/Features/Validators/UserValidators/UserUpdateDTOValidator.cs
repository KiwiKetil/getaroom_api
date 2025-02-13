using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(16);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(24);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{7,14}$");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
