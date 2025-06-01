using System.Linq.Expressions;

namespace HIV.Interfaces
{
    public interface ICommonOperation<T> where T : class
    {
        Task<List<T>> GetAllAysnc();
        Task<T?> GetByFilterAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> GetByFilterListAsync(Expression<Func<T, bool>> filter);
        Task<T> Create(T dbRecord);
        Task<T> Update(T dbRecord);
        Task<bool> Delete(Expression<Func<T, bool>> filter);
    }
}
