using GenericRepository.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace GenericRepository;

public class BaseRepository<TEntity> : IDisposable
    where TEntity : class
{
    private readonly DbContext _context;

    public BaseRepository(DbContext context)
    {
        _context = context;
    }

    public Task AddAsync(IEnumerable<TEntity> entities, CancellationToken token = default) =>
        GetDbSet(TrackingMode.TrackAll).AddRangeAsync(entities, token);

    public TEntity Update(TEntity entity) => GetDbSet(TrackingMode.TrackAll).Update(entity).Entity;

    public void Update(IEnumerable<TEntity> entities) => GetDbSet(TrackingMode.TrackAll).UpdateRange(entities);

    public void PhysicalDelete(TEntity entity) => GetDbSet(TrackingMode.TrackAll).Remove(entity);

    public void PhysicalDelete(IEnumerable<TEntity> entities) => GetDbSet(TrackingMode.TrackAll).RemoveRange(entities);

    public virtual Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        return _context.SaveChangesAsync(token);
    }

    protected DbSet<TEntity> GetDbSet(TrackingMode trackingMode = TrackingMode.NoTracking)
    {
        _context.ChangeTracker.QueryTrackingBehavior = (trackingMode == TrackingMode.TrackAll)
            ? QueryTrackingBehavior.TrackAll
            : QueryTrackingBehavior.NoTracking;

        return _context.Set<TEntity>();
    }

    protected IQueryable<TEntity> GetBaseQuery(TrackingMode trackingMode = TrackingMode.NoTracking) =>
        GetDbSet(trackingMode);

    protected IQueryable<TEntity> GetBaseQuery(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = GetBaseQuery(trackingMode);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }

    protected IQueryable<TEntity> GetBaseQueryWithIncludes(TrackingMode trackingMode = TrackingMode.NoTracking,
        params Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>[] includes)
    {
        var query = GetBaseQuery(trackingMode);

        foreach (var include in includes)
        {
            query = include(query);
        }

        return query;
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}