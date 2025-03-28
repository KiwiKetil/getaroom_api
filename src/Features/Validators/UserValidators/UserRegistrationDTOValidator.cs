﻿using FluentValidation;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;

namespace GetARoomAPI.Features.Validators.UserValidators;

public class UserRegistrationDTOValidator : AbstractValidator<ClientRegistrationDTO>
{
    private const string PasswordPattern = @"^(?=.*[0-9])(?=.*[A-Z])(?=.*[a-z])(?=.*[!?*#_-]).{8,24}$";

    public UserRegistrationDTOValidator()
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

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(PasswordPattern).WithMessage("Password must be 8-24 characters, include at least 1 number, 1 uppercase," +
                                                  " 1 lowercase, and 1 special character ('! ? * # _ -')");
    }
}
