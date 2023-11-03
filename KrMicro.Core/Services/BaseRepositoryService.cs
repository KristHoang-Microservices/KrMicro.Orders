using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Core.Services;

public interface IBaseService<TBaseEntity> where TBaseEntity : class, new()
{
    Task<TBaseEntity> InsertAsync(TBaseEntity entity);
    Task<TBaseEntity> AttackAsync(TBaseEntity entity);
    Task<IEnumerable<TBaseEntity>> GetAllAsync();

    Task<TBaseEntity?> GetDetailAsync(Expression<Func<TBaseEntity, bool>> predicate);

    Task<bool> CheckExistsAsync(Expression<Func<TBaseEntity, bool>> predicate);
    Task<TBaseEntity> UpdateAsync(TBaseEntity entity);
    Task<bool> DeleteAsync(TBaseEntity entity);
}

public class BaseRepositoryService<TEntity, TDbContext> : IBaseService<TEntity>
    where TEntity : class, new() where TDbContext : DbContext, new()
{
    protected readonly TDbContext DataContext;

    public BaseRepositoryService(TDbContext dataContext)
    {
        DataContext = dataContext;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(InsertAsync)} entity must not be null");

        try
        {
            await DataContext.AddAsync(entity);
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be saved: {ex.Message}");
        }
    }

    public async Task<TEntity> AttackAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");

        try
        {
            DataContext.Attach(entity);
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
        }
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");

        try
        {
            DataContext.Update(entity);
            DataContext.Entry(entity).State = EntityState.Modified;
            await DataContext.SaveChangesAsync();

            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be updated {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException($"{nameof(DeleteAsync)} entity must not be null");

        try
        {
            DataContext.Remove(entity);
            await DataContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"{nameof(entity)} could not be removed {ex.Message}");
        }
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            return await DataContext.Set<TEntity>()
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }

    public async Task<TEntity?> GetDetailAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var  result = await DataContext.Set<TEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);

            return result ?? null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }

    public async Task<bool> CheckExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await DataContext.Set<TEntity>()
                .AsNoTracking()
                .AnyAsync(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Couldn't retrieve entities: {ex.Message}");
        }
    }
}