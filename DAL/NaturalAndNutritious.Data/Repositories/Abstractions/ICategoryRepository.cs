﻿using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId);
        Task<IQueryable<SubCategory>> GetSubCategoriesByCategoryId(Guid categoryId);
    }
}
