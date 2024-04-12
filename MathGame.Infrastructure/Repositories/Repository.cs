using MathGame.Core.Entities;
using MathGame.Infrastructure.Context.Interface;
using Microsoft.EntityFrameworkCore;

namespace MathGame.Infrastructure.Repositories;
public class Repository<T> where T : TrackedTable 
{
    protected readonly IMathGameContext _mathGameContext;
    protected readonly DbSet<T> _dbSet;


    public Repository(IMathGameContext mathGameContext)
    {
        _mathGameContext = mathGameContext;
         _dbSet = _mathGameContext.Set<T>();
    }
    
    public IQueryable<T> All()
    {
        return _dbSet.Where(x => x.DeletedOn == null).AsQueryable();
    }

    public IQueryable<T> AllNoTrackedOf()
    {
        return _dbSet.Where(x => x.DeletedOn == null).AsNoTracking().AsQueryable();
    }

    public IQueryable<F>  All<F>() where F : TrackedTable
    {
        return _mathGameContext.Set<F>().Where(x => x.DeletedOn == null).AsQueryable();
    }

    public IQueryable<F> AllNoTrackedOf<F>() where F : TrackedTable
    {
        return _mathGameContext.Set<F>().AsNoTracking().Where(x => x.DeletedOn == null).AsQueryable();
    }

    public async Task<T> Find(int id)
    {
        return await _dbSet.Where(x => x.DeletedOn == null && x.Id == id).SingleOrDefaultAsync();
    }

    public async Task<T> Find(Guid uid)
    {
        return await _dbSet.Where(x => x.DeletedOn == null && x.Uid == uid).SingleOrDefaultAsync();
    }

    public async Task SoftDelete(T entity)
    {
        entity.DeletedOn = DateTime.UtcNow;
        await SaveAsync();
    }

    public void Insert(T entity)
    {
        entity.CreatedOn = DateTime.UtcNow;
        _dbSet.Add(entity); 
    }

    public async Task SaveAsync()
    {
        await _mathGameContext.SaveAsync();
    }
}
