using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IEmailService
    {
        Task SendAsync(MailDto mailDto);
    }
}
