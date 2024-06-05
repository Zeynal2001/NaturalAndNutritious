using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Repositories
{
    public class BlogRepository : Repository<Blog>, IBlogRepository
    {
        public BlogRepository(AppDbContext context) : base(context)
        {
        }
    }
}
