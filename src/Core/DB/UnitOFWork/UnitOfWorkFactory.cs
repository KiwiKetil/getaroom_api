using GetARoomAPI.Core.DB.DBConnection.Interface;
using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;

namespace GetARoomAPI.Core.DB.UnitOFWork;

public class UnitOfWorkFactory(IDbConnectionFactory dbConnectionFactory) : IUnitOfWorkFactory
{
    private readonly IDbConnectionFactory _dbConnectionFactory = dbConnectionFactory;

    public IUnitOfWork Create()
    {
        return new UnitOFWork(_dbConnectionFactory);
    }
}
