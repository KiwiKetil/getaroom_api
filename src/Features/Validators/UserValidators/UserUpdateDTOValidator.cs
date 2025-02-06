using FluentValidation;
using RoomSchedulerAPI.Features.Models.DTOs;
using System.Text.RegularExpressions;

namespace RoomSchedulerAPI.Features.Validators.UserValidators;

public class UserUpdateDTOValidator : AbstractValidator<UserUpdateDTO>
{
    public UserUpdateDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .NotEqual("string")
            .MaximumLength(16);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .NotEqual("string")
            .MaximumLength(24);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{7,14}$"); 

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
