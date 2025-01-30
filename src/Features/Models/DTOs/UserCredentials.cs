namespace RoomSchedulerAPI.Features.Models.DTOs;

public record UserCredentials
(
    string HashedPassword,
    string Salt
);
