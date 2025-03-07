using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;
using System.Data;

namespace RoomSchedulerAPI.Core.DB.UnitOFWork;

public record struct UnitofWorkId(Guid Value) 
{
    public static UnitofWorkId NewId => new (Guid.NewGuid());
    public static UnitofWorkId Empty => new (Guid.Empty); 
}

public class UnitOFWork(IDbConnectionFactory dbConnectionFactory) : IUnitOfWork
{
    private readonly UnitofWorkId _id = UnitofWorkId.NewId;
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;

    public UnitofWorkId Id => _id;
    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection has not been initialized.");
    public IDbTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction has not been initialized.");

    public async Task BeginAsync()
    {
        _connection = await _dbConnectionFactory.CreateConnectionAsync();
        _transaction = _connection.BeginTransaction();
    }

    public Task CommitAsync()
    {
        Transaction.Commit();
        Connection.Close();
        return Task.CompletedTask;
    }  

    public Task RollbackAsync()
    {
        Transaction.Rollback();
        Connection.Close();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}
