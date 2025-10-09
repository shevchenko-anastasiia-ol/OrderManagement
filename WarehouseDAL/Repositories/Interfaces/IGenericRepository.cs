using System.Linq.Expressions;
using Ardalis.Specification;
using WarehouseBLL.Specifications;

namespace WarehouseDAL.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec);
    Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> spec);
}