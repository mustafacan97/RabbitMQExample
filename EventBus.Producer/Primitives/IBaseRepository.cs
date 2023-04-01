using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ProducerService.Primitives;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    #region Public Methods

    bool Exists(Expression<Func<TEntity, bool>>? selector = null);

    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null);

    void Insert(params TEntity[] entities);

    TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>>? predicate = null,
                              Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                              Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                              bool disableTracking = true,
                              bool ignoreQueryFilters = false);

    #endregion
}
