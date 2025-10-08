using Microsoft.EntityFrameworkCore;
using WarehouseDAL.Data;
using WarehouseDAL.Repositories.Interfaces;

namespace WarehouseDAL.Interfaces;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly WarehouseDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public GenericRepository(WarehouseDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<T>();
    }
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}