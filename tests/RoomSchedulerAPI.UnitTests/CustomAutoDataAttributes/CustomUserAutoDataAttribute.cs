using AutoFixture;
using AutoFixture.Xunit2;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.UnitTests.CustomAutoDataAttributes;
internal class CustomUserAutoDataAttribute : AutoDataAttribute
{
    public CustomUserAutoDataAttribute() : base(() =>
    {
        var fixture = new Fixture();

        fixture.Customize<User>(c => c
        .With(x => x.Id, new UserId(new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d481")))
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        fixture.Customize<List<User>>(c => c.FromFactory(() =>
        [
            new() { FirstName = "Jim", LastName = "Moore" },
            new() { FirstName = "Michelle", LastName = "Andersson" }
        ]));

        fixture.Customize<UserDTO>(u => u
        .With(x => x.Id, new UserId(new Guid("a47ac10b-58cc-4372-a567-0e02b2c3d481")))
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        fixture.Customize<List<UserDTO>>(c => c.FromFactory(() =>
        [
            new(UserId.NewId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", []),
            new(UserId.NewId, "Kristoffer", "Sveberg", "91918262", "kristoffersveberg@gmail.com", []),
            new(UserId.NewId, "lara", "Sveberg", "92628191", "larasveberg@gmail.com", [])
        ]));

        fixture.Customize<UserRegistrationDTO>(u => u
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        fixture.Customize<UserUpdateDTO>(u => u
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        return fixture;
    })
    {
    }
}
