namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IAuthorizableDTO
{
    string Email { get; init; }
    string Password { get; init;  }
}
