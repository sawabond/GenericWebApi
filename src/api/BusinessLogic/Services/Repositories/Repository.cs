using DataAccess;
using DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BusinessLogic.Services.Repositories;

internal abstract class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity<string>
{
    public Repository(ApplicationContext context)
    {
        Context = context;
    }

    protected ApplicationContext Context { get; }

    private DbSet<TEntity> Entities => Context.Set<TEntity>();

    public async Task AddAsync(TEntity entity)
    {
        await Entities.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await Entities.AddRangeAsync(entities);
    }

    public async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression)
    {
        return Entities.Where(expression);
    }

    public async Task<IQueryable<TEntity>> GetAllAsync()
    {
        return Entities.AsQueryable();
    }

    public async Task<TEntity> GetAsync(string id)
    {
        return await Entities.FindAsync(id);
    }

    public void Remove(TEntity entity)
    {
        Entities.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        Entities.RemoveRange(entities);
    }

    public void Update(TEntity entity)
    {
        Entities.Update(entity);
    }
}
