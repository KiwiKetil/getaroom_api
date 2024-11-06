using Dapper;
using RoomSchedulerAPI.Features.Models.Entities;
using System.Data;

namespace RoomSchedulerAPI.Core.DB.TypeHandlers
{
    public class UserIdHandler : SqlMapper.TypeHandler<UserId>
    {
        // from db
        public override UserId Parse(object value)  // obj value is value from db
        {
            if (value is Guid guidValue)
            {
                return new UserId(guidValue); 
            }
            throw new DataException($"Cannot convert {value.GetType()} to UserId.");
        }

        // to db
        public override void SetValue(IDbDataParameter parameter, UserId value)
        {
            parameter.Value = value.Value; 
            parameter.DbType = DbType.Guid;
        }
    }
}
