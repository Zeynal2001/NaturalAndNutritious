using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        Task<IQueryable<Product>> GetProductsBySubCategoryId(Guid subCategoryId);
        Task<Category> GetCategoryBySubCategoryId(Guid subCategoryId);
    }
}
