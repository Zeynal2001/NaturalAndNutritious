using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity, new()
    {
        public Repository(AppDbContext context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;
        
        public DbSet<T> Table => _context.Set<T>();

        public async Task CreateAsync(T entity)
        {
           await Table.AddAsync(entity);
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.FromResult(Table);
            //return await Task.Run(() => Table);

            //if (entity == null)
            //{
            //    Enumerable.Empty<T>();
            //}
        }

        public async Task<IQueryable<T>> FilterWithPagination(int page, int size)
        {
            if (page == 0 && size == 0)
            {
                return await Task.FromResult(Table.Where(c => c.IsDeleted == false));
            }

            return await Task.FromResult(Table.Skip((page - 1) * size).Take(size)
                .Where(c => c.IsDeleted == false));
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var entity = await Table.FindAsync(id);

            return entity;
        }

        public async Task<bool> UpdateAsync(T updatedEntity)
        {
            var entity = await Table.FindAsync(updatedEntity.Id);

            if (entity == null)
            {
                //verilen Id ye gore obyekt tapilmasa false qaytarilir.
                return false;
            }

            //_context.Entry(entity).CurrentValues.SetValues(updatedEntity);
            
            Table.Update(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await Table.FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            Table.Remove(entity);

            return true;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<int> TotalCountAsync(T entity)
        {
            return await Table.CountAsync();
        }


        //public async Task<int> SaveChangesAsync()
        //{
        //    return await _context.SaveChangesAsync();
        //}
    }
}
