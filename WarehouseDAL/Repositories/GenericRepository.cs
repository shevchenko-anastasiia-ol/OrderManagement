using System.Linq.Expressions;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WarehouseBLL.Specifications;
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
    
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
    
    public async Task<List<T>> ListAsync(ISpecification<T> spec)
    {
        var specificationResult = ApplyArdalisSpecification(spec);
        return await specificationResult.ToListAsync();
    }
    
    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var specificationResult = ApplyArdalisSpecification(spec);
        return await specificationResult.CountAsync();
    }
    
    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec)
    {
        var specificationResult = ApplyArdalisSpecification(spec);
        return await specificationResult.FirstOrDefaultAsync();
    }

    public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> spec)
    {
        var specificationResult = ApplyArdalisSpecification(spec);
        return await specificationResult.SingleOrDefaultAsync();
    }

    private IQueryable<T> ApplyArdalisSpecification(ISpecification<T> spec)
    {
        var evaluator = new SpecificationEvaluator();
        return evaluator.GetQuery(_dbSet, spec);
    }
    
    
}