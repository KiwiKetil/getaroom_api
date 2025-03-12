using Microsoft.AspNetCore.Mvc;

namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UserQuery
(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Email,
    string SortBy = "LastName",
    string Order = "ASC",
    int Page = 1,
    int PageSize = 10,
    [FromQuery] string[]? Roles = null
    );
