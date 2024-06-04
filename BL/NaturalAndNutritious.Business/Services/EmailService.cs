using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.OptionTypes;

namespace NaturalAndNutritious.Business.Services
{
    public class EmailService : IEmailService
    {
        //mime
        public EmailService(EmailOptions options)
        {
            _options = options;
        }

        private readonly EmailOptions _options;
        public async Task SendAsync(MailDto dto)
        {
            var mime = CreateMimeMessage(dto);

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(host: _options.Host, port: _options.Port, true);
                await client.AuthenticateAsync(userName: _options.User, password: _options.Pass);
                await client.SendAsync(mime);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private MimeMessage CreateMimeMessage(MailDto dto)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_options.Name, _options.From));
            mime.To.AddRange(dto.Addresses);
            mime.Subject = dto.Subject;
            mime.Body = new TextPart(TextFormat.Text) { Text = dto.Content };

            return mime;
        }
    }
}
