namespace GetARoomAPI.Features.Models.DTOs.ResponseDTOs;

public record ErrorResponse
(
    string? Message = "",
    List<string>? Errors = null
);
