using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using ProducerService.Data;

namespace ProducerService.Primitives;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    #region Fields

    private readonly ProducerServiceDbContext _dbContext;

    private readonly DbSet<TEntity> _dbSet;

    #endregion

    #region Constructors and Destructors

    public BaseRepository(ProducerServiceDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    #endregion

    #region Public Methods

    public bool Exists(Expression<Func<TEntity, bool>>? selector = null)
    {
        return selector == null ? _dbSet.Any() : _dbSet.Any(selector);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null)
    {
        return selector == null ? await _dbSet.AnyAsync() : await _dbSet.AnyAsync(selector);
    }

    public void Insert(params TEntity[] entities)
    {
        _dbSet.AddRange(entities);
        _dbContext.SaveChanges();
    }

    public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null,
                                     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                     Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                     bool disableTracking = true,
                                     bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
            query = query.AsNoTracking();

        if (include != null)
            query = include(query);

        if (predicate != null)
            query = query.Where(predicate);

        if (ignoreQueryFilters)
            query = query.IgnoreQueryFilters();

        if (orderBy != null)
            return orderBy(query).FirstOrDefault();

        return query.FirstOrDefault();
    }

    #endregion
}
