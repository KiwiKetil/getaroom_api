using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
public record LoginDTO(string Email, string Password) : IVerifyUserCredentials;
