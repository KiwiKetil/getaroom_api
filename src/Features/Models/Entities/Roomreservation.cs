
namespace RoomSchedulerAPI.Features.Models.Entities;

public readonly record struct RoomReservationId(Guid Value)
{
    public static RoomReservationId NewId => new(Guid.NewGuid());
    public static RoomReservationId Empty => new(Guid.Empty);
};

public class Roomreservation
{
    public RoomReservationId Id { get; set; }
    public UserId UserId { get; set; }
    public RoomId RoomId { get; set; }
    public string? ClientInfo { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}