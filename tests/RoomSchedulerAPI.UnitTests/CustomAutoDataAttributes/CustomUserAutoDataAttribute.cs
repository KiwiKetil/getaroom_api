using AutoFixture;
using AutoFixture.Xunit2;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.UnitTests.CustomAutoDataAttributes;
internal class CustomUserAutoDataAttribute : AutoDataAttribute
{
    public CustomUserAutoDataAttribute() : base(() =>
    {
        var fixture = new Fixture();

        fixture.Customize<User>(c => c
        .With(x => x.Id, UserId.NewId)
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        fixture.Customize<UserRegistrationDTO>(u => u
        .With(x => x.FirstName, "Al")
        .With(x => x.LastName, "Bundy")
        .With(x => x.PhoneNumber, "76544567")
        .With(x => x.Email, "al@bundy.com"));

        return fixture;
    })
    { 
    }
}
