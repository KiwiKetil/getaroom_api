using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Repositories.Interfaces;

public interface IPasswordHistoryRepository
{
    Task<bool> PasswordUpdateExistsAsync(UserId id);    
    Task<bool> InsertPasswordUpdateRecordAsync(Guid userId, IUnitOfWork unitOfWork);
}
