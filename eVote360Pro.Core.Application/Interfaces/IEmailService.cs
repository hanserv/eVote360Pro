using eVote360Pro.Core.Application.DTOs.Email;

namespace eVote360Pro.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendAsync(EmailRequestDto emailRequestDto);
    }
}
