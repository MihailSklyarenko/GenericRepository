using GenericRepository.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository;

public class BaseRepository<TContext> where TContext : DbContext
{
    private readonly TContext _context;
    public BaseRepository(TContext context)
    {
        _context = context;
    }

    public Task<TEntity[]> SelectByConditionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetBaseQuery<TEntity>(tracking)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQuery(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity[]> SelectByConditionAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .Where(predicate)
            .ToArrayAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetBaseQuery<TEntity>(tracking)
            .Where(predicate)
            .FirstOrDefaultAsync(token);
    }

    public Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQuery(tracking, includes)
            .FirstOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> FirstOrDefaultIncludeAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .FirstOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> SingleOrDefaultAsync<TEntity>(TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQuery(tracking, includes)
            .SingleOrDefaultAsync(token);
    }

    public Task<TEntity?> SingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQuery(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<TEntity?> SingleOrDefaultIncludeAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        TrackingMode tracking = TrackingMode.NoTracking,
        CancellationToken token = default,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        where TEntity : class
    {
        return GetBaseQueryWithIncludes(tracking, includes)
            .SingleOrDefaultAsync(predicate, token);
    }

    public Task<int> CountAsync<TEntity>(CancellationToken token = default)
        where TEntity : class
    {
        return GetDBset<TEntity>(TrackingMode.NoTracking)
            .CountAsync(token);
    }

    public Task<bool> AnyAsync<TEntity>(CancellationToken token = default) where TEntity : class
    {
        return GetDBset<TEntity>(TrackingMode.NoTracking)
            .AnyAsync(token);
    }

    public Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetDBset<TEntity>(TrackingMode.NoTracking)
            .AnyAsync(predicate, token);
    }

    public Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetDBset<TEntity>(TrackingMode.NoTracking)
            .CountAsync(predicate, token);
    }

    private IQueryable<TEntity> GetBaseQuery<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking)
        where TEntity : class => GetDBset<TEntity>();

    private IQueryable<TEntity> GetBaseQuery<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
        where TEntity : class
    {
        var query = GetBaseQuery<TEntity>(trackingMode);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return query;
    }

    private IQueryable<TEntity> GetBaseQueryWithIncludes<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
        where TEntity : class
    {
        var query = GetBaseQuery<TEntity>(trackingMode);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = include(query);
            }
        }

        return query;
    }


    private DbSet<TEntity> GetDBset<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking) where TEntity : class
    {
        _context.ChangeTracker.QueryTrackingBehavior = (trackingMode == TrackingMode.TrackAll)
            ? QueryTrackingBehavior.TrackAll
            : QueryTrackingBehavior.NoTracking;

        return _context.Set<TEntity>();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}