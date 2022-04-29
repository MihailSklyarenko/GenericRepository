using GenericRepository.Enums;
using GenericRepository.Models.Sorting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using GenericRepository.Enums.Sorting;
using System.Reflection;

namespace GenericRepository;

public class BaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    public BaseRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default)
    {
        var task = await GetDbSet(TrackingMode.TrackAll).AddAsync(entity, token);
        return task.Entity;
    }

    public Task AddAsync(IEnumerable<TEntity> entities, CancellationToken token = default) => GetDbSet(TrackingMode.TrackAll).AddRangeAsync(entities, token);

    public TEntity Update(TEntity entity) => GetDbSet(TrackingMode.TrackAll).Update(entity).Entity;

    public void Update(IEnumerable<TEntity> entities) => GetDbSet(TrackingMode.TrackAll).UpdateRange(entities);

    public void PhysicalDelete(TEntity entity) => GetDbSet(TrackingMode.TrackAll).Remove(entity);

    public void PhysicalDelete(IEnumerable<TEntity> entities) => GetDbSet(TrackingMode.TrackAll).RemoveRange(entities);

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default)
    {
        return GetBaseQuery(tracking)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default)
    {
        var filteredQuery = GetBaseQuery(tracking).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);

        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var filteredQuery = GetBaseQuery(tracking, includes).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);
        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        var filteredQuery = GetBaseQueryWithIncludes(tracking, includes).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);
        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default)
    {
        return GetBaseQuery(tracking)
            .Where(predicate)
            .FirstOrDefaultAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .FirstOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> FirstOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .FirstOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> SingleOrDefaultAsync(TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .SingleOrDefaultAsync(token);
    }

    public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> SingleOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<int> CountAsync(CancellationToken token = default) => GetDbSet().CountAsync(token);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) => GetDbSet().CountAsync(predicate, token);
    
    public Task<bool> AnyAsync(CancellationToken token = default) => GetDbSet().AnyAsync(token);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) => GetDbSet().AnyAsync(predicate, token);

    public virtual Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync(token);
    }

    private IQueryable<TEntity> GetBaseQuery(TrackingMode trackingMode = TrackingMode.NoTracking) => GetDbSet(trackingMode);

    private IQueryable<TEntity> GetBaseQuery(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetBaseQuery(trackingMode);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return query;
    }

    private IQueryable<TEntity> GetBaseQueryWithIncludes(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        var query = GetBaseQuery(trackingMode);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = include(query);
            }
        }

        return query;
    }

    private DbSet<TEntity> GetDbSet(TrackingMode trackingMode = TrackingMode.NoTracking)
    {
        _context.ChangeTracker.QueryTrackingBehavior = (trackingMode == TrackingMode.TrackAll)
            ? QueryTrackingBehavior.TrackAll
            : QueryTrackingBehavior.NoTracking;

        return _context.Set<TEntity>();
    }

    private IQueryable<TEntity> Sort(IQueryable<TEntity> filteredQuery, IEnumerable<SortingParameter> sortings)
    {
        var orderedQuery = filteredQuery;
        if (sortings?.Any() == true)
        {
            var type = typeof(TEntity);
            var orderByCommand = sortings.First().Direction == SortDirection.Ascending
                ? "OrderBy"
                : "OrderByDescending";

            var isOrderedQuery = false;
            foreach (var sorting in sortings)
            {
                var property =
                    type.GetProperty(sorting.FieldName, BindingFlags.FlattenHierarchy
                    | BindingFlags.Instance
                    | BindingFlags.Public);

                if (property == null)
                {
                    throw new Exception($"Property {sorting.FieldName} is not found");
                }
                if (isOrderedQuery)
                {
                    orderByCommand = sorting.Direction == SortDirection.Ascending
                        ? "ThenBy"
                        : "ThenByDescending";
                }
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);
                var resultExpression = Expression.Call(typeof(Queryable), orderByCommand, new Type[] { type, property.PropertyType },
                    orderedQuery.Expression, Expression.Quote(orderByExpression));
                orderedQuery = orderedQuery.Provider.CreateQuery<TEntity>(resultExpression);
                isOrderedQuery = true;
            }
        }
        return orderedQuery;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}