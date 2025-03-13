using GetARoomAPI.Features.Services.Interfaces;

namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;
public record LoginDTO(string Username, string Password) : IVerifyUserCredentials;
