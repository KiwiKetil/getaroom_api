namespace RoomSchedulerAPI.Features.Models.DTOs;

public record UserQuery
(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Email,
    string SortBy = "LastName",
    string Order = "ASC",
    int Page = 1,
    int PageSize = 10    
);
