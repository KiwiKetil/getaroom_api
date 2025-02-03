using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface ITokenGenerator
{
    Task<string> GenerateTokenAsync(User user);
}
