using GenericRepository.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository;

public class BaseRepository<TContext, TEntity> where TContext : DbContext
{
    private readonly TContext _context;
    public BaseRepository(TContext context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity, 
        CancellationToken token = default) 
        where TEntity : class
    {
        var task = await GetDbSet<TEntity>(TrackingMode.TrackAll)
            .AddAsync(entity);
        return task.Entity;
    }

    public Task AddAsync<TEntity>(IEnumerable<TEntity> entities, 
        CancellationToken token = default) where TEntity : class => 
            GetDbSet<TEntity>(TrackingMode.TrackAll).AddRangeAsync(entities, token);

    public TEntity Update<TEntity>(TEntity entity) where TEntity : class => 
        GetDbSet<TEntity>(TrackingMode.TrackAll).Update(entity).Entity;

    public void Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class =>
        GetDbSet<TEntity>(TrackingMode.TrackAll).UpdateRange(entities);

    public void PhysicalDelete<TEntity>(TEntity entity) where TEntity : class => 
        GetDbSet<TEntity>(TrackingMode.TrackAll).Remove(entity);

    public void PhysicalDelete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class =>
        GetDbSet<TEntity>(TrackingMode.TrackAll).RemoveRange(entities);

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
        return GetDbSet<TEntity>(TrackingMode.NoTracking)
            .CountAsync(token);
    }

    public Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetDbSet<TEntity>(TrackingMode.NoTracking)
            .CountAsync(predicate, token);
    }

    public Task<bool> AnyAsync<TEntity>(CancellationToken token = default) where TEntity : class
    {
        return GetDbSet<TEntity>(TrackingMode.NoTracking)
            .AnyAsync(token);
    }

    public Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate,
        CancellationToken token = default)
        where TEntity : class
    {
        return GetDbSet<TEntity>(TrackingMode.NoTracking)
            .AnyAsync(predicate, token);
    }

    public virtual Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync(token);
    }

    private IQueryable<TEntity> GetBaseQuery<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking)
        where TEntity : class => GetDbSet<TEntity>();

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

    private DbSet<TEntity> GetDbSet<TEntity>(TrackingMode trackingMode = TrackingMode.NoTracking) where TEntity : class
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