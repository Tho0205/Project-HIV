using System.Linq.Expressions;
using HIV.Interfaces;
using HIV.Models;
using Microsoft.EntityFrameworkCore;
namespace HIV.Repository
{
    public class CommonOperation<T> : ICommonOperation<T> where T : class
    {
        private readonly AppDbContext context;
        private readonly DbSet<T> _dbset;
        public CommonOperation(AppDbContext _context)
        {
            context = _context;
            _dbset = context.Set<T>();
        }
        public async Task<List<T>> GetAllAysnc()
        {
            return await _dbset.ToListAsync();
        }
        public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbset.Where(filter).FirstOrDefaultAsync();
        }
        public async Task<List<T>> GetByFilterListAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbset.Where(filter).ToListAsync();
        }
        public async Task<T> Create(T dbRecord)
        {
            await _dbset.AddAsync(dbRecord);
            await context.SaveChangesAsync();
            return dbRecord;
        }
        public async Task<T> Update(T dbRecord)
        {
           _dbset.Update(dbRecord);
            await context.SaveChangesAsync();
            return dbRecord;
        }
        public async Task<bool> Delete(Expression<Func<T, bool>> filter)
        {
            var recordToDelete = await _dbset.Where(filter).FirstOrDefaultAsync();
            if(recordToDelete == null)
            {
                return false;
            }
            _dbset.Remove(recordToDelete);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
