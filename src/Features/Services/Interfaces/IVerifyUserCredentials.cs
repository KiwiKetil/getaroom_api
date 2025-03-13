namespace GetARoomAPI.Features.Services.Interfaces;

public interface IVerifyUserCredentials
{
    string Username { get; init; }
    string Password { get; init; }
}
