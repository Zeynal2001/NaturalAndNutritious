using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface IUserRepository
    {
        Task<IQueryable<Order>> GetOrdersByUserId(string userId);
        Task<IQueryable<Review>> GetReviewsByUserId(string userId);

    }
}
