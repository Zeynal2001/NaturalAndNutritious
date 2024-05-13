using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        DbSet<TEntity> Table { get; }
        Task<IQueryable<TEntity>> GetAllAsync();
        IQueryable<TEntity> FilterWithPagination(int page = 0, int size = 0);
        Task<TEntity?> GetByIdAsync(Guid id);
        Task CreateAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity updatedEntity);
        Task<bool> DeleteAsync(Guid id);
        Task<int> SaveChangesAsync();
    }
}
