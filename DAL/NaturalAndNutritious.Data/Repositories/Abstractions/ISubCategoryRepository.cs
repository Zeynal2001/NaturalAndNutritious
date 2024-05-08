using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        Task<IQueryable<Product>> GetProductsBySubCategoryId(Guid subCategoryId);
        Task<Category> GetCategoryBySubCategoryId(Guid subCategoryId);

    }
}
