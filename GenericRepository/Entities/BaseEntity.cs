namespace GenericRepository.Entities;

public class BaseEntity<TId> : IEntity<TId> where TId : struct 
{
    public TId Id { get ; set ; }
}