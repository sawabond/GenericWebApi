using System.Linq.Expressions;

namespace DataAccess.Abstractions;

public interface IRepository<TEntity> where TEntity : IEntity<string>
{
    Task<TEntity> GetAsync(string id);

    Task<IQueryable<TEntity>> GetAllAsync();

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveRange(IEnumerable<TEntity> entities);
}
