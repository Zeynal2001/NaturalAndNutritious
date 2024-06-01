using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId);
        Task<IQueryable<SubCategory>> GetSubCategoriesByCategoryId(Guid categoryId);
    }
}
