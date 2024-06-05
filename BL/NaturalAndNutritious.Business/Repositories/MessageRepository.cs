using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Repositories
{
    public class MessageRepository : Repository<ContactMessage>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
