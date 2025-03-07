using System.Data;

namespace RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;

public interface IUnitOfWork : IDisposable
{
    UnitofWorkId Id { get; }
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }
    Task BeginAsync();
    Task CommitAsync();
    Task RollbackAsync();
}