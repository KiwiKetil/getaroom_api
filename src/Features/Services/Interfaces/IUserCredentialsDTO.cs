namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserCredentialsDTO
{
    string Email { get; init; }
    string Password { get; init;  }
}
