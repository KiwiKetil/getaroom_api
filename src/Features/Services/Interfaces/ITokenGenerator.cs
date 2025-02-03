namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface ITokenGenerator
{
    Task<string> GenerateJWTAsync();
}
