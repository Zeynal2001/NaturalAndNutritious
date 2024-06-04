using MimeKit;

namespace NaturalAndNutritious.Business.Dtos
{
    public class MailDto
    {
        public List<MailboxAddress> Addresses { get; set; } //or called Recipients
        public string Subject { get; set; } //what will be the message
        public string Content { get; set; }
    }
}