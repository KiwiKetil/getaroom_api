using Dapper;
using RoomSchedulerAPI.Features.Models.Entities;
using System.Data;

namespace RoomSchedulerAPI.Core.DB.TypeHandlers;

public class UserIdHandler : SqlMapper.TypeHandler<UserId>
{
    public override UserId Parse(object value)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(IDbDataParameter parameter, UserId value)
    {
        throw new NotImplementedException();
    }
}
