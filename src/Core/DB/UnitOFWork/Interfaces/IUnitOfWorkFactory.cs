namespace RoomSchedulerAPI.Core.DB.UnitOFWork.Interfaces;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}
