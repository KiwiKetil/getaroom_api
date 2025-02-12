using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IPasswordHistoryRepository
{
    Task<bool> PasswordChangeExistsAsync(UserId id);

    Task InsertPasswordChangeRecordAsync(Guid userId);
}
