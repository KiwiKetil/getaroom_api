
namespace RoomSchedulerAPI.Features.Models.Entities;

public readonly record struct RoomreservationId(Guid roomReservationId)
{
    public static RoomreservationId NewId => new(Guid.NewGuid());
    public static RoomreservationId Empty => new(Guid.Empty);
};

public class Roomreservation
{
    public RoomreservationId Id { get; set; }
    public UserId UserId { get; set; }
    public RoomId RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}