using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IPasswordHistoryRepository
{
    Task<bool> PasswordUpdateExistsAsync(UserId id);

    Task InsertPasswordUpdateRecordAsync(Guid userId);
}
