
namespace RoomSchedulerAPI.Features.Models.Entities;

public readonly record struct RoomId(Guid roomId)
{
    public static RoomId NewId => new(Guid.NewGuid());
    public static RoomId Empty => new(Guid.Empty);
};

public class Room
{
    public RoomId Id { get; set; }
    public string? RoomName { get; set; }
    public sbyte Capacity { get; set; }
    public bool MonitorAvailable { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}