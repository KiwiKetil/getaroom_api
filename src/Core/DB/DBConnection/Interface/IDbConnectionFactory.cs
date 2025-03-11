using System.Data;

namespace GetARoomAPI.Core.DB.DBConnection.Interface;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<IDbConnection> CreateConnectionAsync();
}
