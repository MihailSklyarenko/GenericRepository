using GenericRepository.Enums;
using GenericRepository.Enums.Sorting;
using GenericRepository.Models.Sorting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericRepository;

public class Repository<TEntity> : BaseRepository<TEntity>
    where TEntity : class
{
    public Repository(DbContext context) : base(context)
    {}

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken token = default)
    {
        var task = await GetDbSet(TrackingMode.TrackAll).AddAsync(entity, token);
        return task.Entity;
    }
    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking)
    {
        return GetBaseQuery(tracking)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking)
    {
        var filteredQuery = GetBaseQuery(tracking).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);

        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var filteredQuery = GetBaseQuery(tracking, includes).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);
        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        IEnumerable<SortingParameter> sorting,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        var filteredQuery = GetBaseQueryWithIncludes(tracking, includes).Where(predicate);
        var sorteredQuery = Sort(filteredQuery, sorting);
        return sorteredQuery.ToArrayAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking)
    {
        return GetBaseQuery(tracking)
            .Where(predicate)
            .FirstOrDefaultAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .FirstOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> FirstOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
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
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        return GetBaseQuery(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> SingleOrDefaultIncludeAsync(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default,
        TrackingMode tracking = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<int> CountAsync(CancellationToken token = default) => GetDbSet().CountAsync(token);

    public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) =>
        GetDbSet().CountAsync(predicate, token);

    public Task<bool> AnyAsync(CancellationToken token = default) => GetDbSet().AnyAsync(token);

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token = default) =>
        GetDbSet().AnyAsync(predicate, token);

    private IQueryable<TEntity> Sort(IQueryable<TEntity> query, IEnumerable<SortingParameter> sortingParameters)
    {
        var result = query;
        if (sortingParameters?.Any() == true)
        {
            var type = typeof(TEntity);
            var orderByCommand = sortingParameters.First().Direction == SortDirection.Ascending
                ? "OrderBy"
                : "OrderByDescending";

            var isOrderedQuery = false;
            foreach (var sorting in sortingParameters)
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
                    result.Expression, Expression.Quote(orderByExpression));
                result = result.Provider.CreateQuery<TEntity>(resultExpression);
                isOrderedQuery = true;
            }
        }
        return result;
    }
}