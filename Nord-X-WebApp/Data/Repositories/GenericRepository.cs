using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Nord_X_WebApp.Data.Interfaces;
using System.Linq.Expressions;

namespace Nord_X_WebApp.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string[]? includeProperties = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter is not null)
                query = query.Where(filter);

            if (includeProperties is not null)
                foreach (var includeProperty in includeProperties)
                    query = query.Include(includeProperty);

            if (orderBy is not null)
                return await orderBy(query).ToListAsync();

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetAsync(Guid? id, string[]? includeProperties = null)
        {
            if (id is null)
                // TODO: Maybe better to log event and return null - null is already an accepted return value of this method.
                //throw new ArgumentNullException($"No argument {nameof(id)} was given to find {this.GetType().Name} entity.");
                return null;

            var entity = await _dbSet.FindAsync(id);
            if (entity is null)
                return null;

            if (includeProperties is not null)
                foreach (var includeProperty in includeProperties)
                {
                    var dbMemberEntry = _context.Entry(entity).Member(includeProperty);
                    if (dbMemberEntry is CollectionEntry collectionEntry)
                        await collectionEntry.LoadAsync();

                    if (dbMemberEntry is ReferenceEntry referenceEntry)
                        await referenceEntry.LoadAsync();
                }

            return entity;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual EntityEntry<TEntity> Delete(TEntity entity)
        {
            return _dbSet.Remove(entity);
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetAsync(id);
            if (entity is null)
                return false;
            return (Delete(entity) is not null);
        }

        public virtual bool Update(TEntity entity)
        {
            if (entity is null)
                return false;
            _dbSet.Update(entity);
            return true;
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbSet.AnyAsync(filter);
        }
    }
}
