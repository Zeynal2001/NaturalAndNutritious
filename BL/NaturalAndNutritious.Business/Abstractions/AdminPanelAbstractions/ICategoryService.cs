﻿using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface ICategoryService
    {
        Task<int> TotalCategories();
        Task<CategoryServiceResult> CreateCategory(CreateCategoryDto model);
    }
}
