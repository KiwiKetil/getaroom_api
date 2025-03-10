using RoomSchedulerAPI.Core.DB.DBConnection.Interface;
using RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;

namespace RoomSchedulerAPI.Core.DB.UnitOFWork;

public class UnitOfWorkFactory(IDbConnectionFactory dbConnectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public IUnitOfWork Create()
    {
        return new UnitOFWork(_dbConnectionFactory);
    }
}
