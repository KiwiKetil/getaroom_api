using System.Data;

namespace RoomSchedulerAPI.Core.DB.DBConnection.Interface;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
