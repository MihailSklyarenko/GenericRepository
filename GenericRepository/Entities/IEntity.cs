namespace GenericRepository.Entities;

public interface IEntity<TId> where TId : struct
{
    public TId Id { get; set; }
}
