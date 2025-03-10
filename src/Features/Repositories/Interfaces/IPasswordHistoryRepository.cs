using RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Repositories.Interfaces;

public interface IPasswordHistoryRepository
{
    Task<bool> PasswordUpdateExistsAsync(UserId id);

    Task<bool> InsertPasswordUpdateRecordAsync(IUnitOfWork uow, Guid userId);
}
