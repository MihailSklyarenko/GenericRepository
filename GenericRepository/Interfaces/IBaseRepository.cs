namespace GenericRepository.Interfaces;

public interface IBaseRepository<TEntity> : IDisposable
    where TEntity : class
{
    Task AddAsync(IEnumerable<TEntity> entities, CancellationToken token = default);

    TEntity Update(TEntity entity);

    void Update(IEnumerable<TEntity> entities);

    void PhysicalDelete(TEntity entity);

    void PhysicalDelete(IEnumerable<TEntity> entities);

    Task<int> SaveChangesAsync(CancellationToken token = default);   
}
