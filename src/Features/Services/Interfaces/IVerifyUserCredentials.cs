namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IVerifyUserCredentials
{
    string Email { get; init; }
    string Password { get; init;  }
}
