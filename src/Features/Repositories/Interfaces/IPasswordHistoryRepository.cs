using RoomSchedulerAPI.Core.DB.UnitOFWork;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IPasswordHistoryRepository
{
    Task<bool> PasswordUpdateExistsAsync(UserId id);

    Task<bool> InsertPasswordUpdateRecordAsync(UnitOFWork uow, Guid userId);
}
