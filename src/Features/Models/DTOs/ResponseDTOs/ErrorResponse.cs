namespace RoomSchedulerAPI.Features.Models.DTOs.ResponseDTOs;

public record ErrorResponse
(
    string? Message,
    List<string>? Errors = null
);
