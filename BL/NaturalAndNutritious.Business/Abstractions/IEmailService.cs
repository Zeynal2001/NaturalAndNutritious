using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(MailDto mailDto);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
    }
}
