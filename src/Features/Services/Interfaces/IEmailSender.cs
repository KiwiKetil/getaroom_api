using RestSharp;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface IEmailSender
{
    Task<RestResponse> SendEmailAsync();
}
