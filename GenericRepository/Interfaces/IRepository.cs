using GenericRepository.Enums;
using GenericRepository.Models.Sorting;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository.Interfaces;

public interface IRepository<TEntity, TKey> : IBaseRepository<TEntity>
    where TEntity : class
    where TKey : struct
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default);

    Task<TEntity?> GetByIdAsync(TKey id,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking);

    Task<TEntity?> GetByIdAsync(TKey id,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByIdIncludeAsync(TKey id,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes);

    Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default, TrackingMode tracking = TrackingMode.NoTracking);

    Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking);

    Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes);

    Task<TEntity[]> SelectByConditionIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> FirstOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes);

    Task<TEntity?> SingleOrDefaultAsync(TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> SingleOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes);

    Task<int> CountAsync(CancellationToken token = default);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);

    Task<bool> AnyAsync(CancellationToken token = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default);    
}