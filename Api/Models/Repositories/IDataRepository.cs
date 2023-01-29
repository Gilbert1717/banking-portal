namespace Api.Models.Repositories;

public interface IDataRepository<TEntity, TKey> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity Get(TKey id);
    void Update( TEntity customer);
}