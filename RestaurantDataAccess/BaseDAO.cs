using System.Linq.Expressions;
using FUBusiness.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public abstract class BaseDAO<TEntity, TId>
        where TEntity : class
    {
        protected readonly RestaurantReservationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        private readonly string _primaryKeyName;

        protected BaseDAO()
        {
            _context = new RestaurantReservationDbContext();
            _dbSet = _context.Set<TEntity>();
            _primaryKeyName = GetPrimaryKeyName();
        }

        private string GetPrimaryKeyName()
        {
            var keyProperty = typeof(TEntity)
                .GetProperties()
                .FirstOrDefault(p =>
                    p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)
                    || p.Name.Equals(
                        $"{typeof(TEntity).Name}Id",
                        StringComparison.OrdinalIgnoreCase
                    )
                    || p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
                );

            return keyProperty?.Name
                ?? throw new InvalidOperationException(
                    $"Entity {typeof(TEntity).Name} must have a primary key named 'Id' or '{typeof(TEntity).Name}Id'."
                );
        }

        protected virtual IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query) => query;

        public virtual async Task<TEntity?> GetByIdAsync(
            TId id,
            CancellationToken cancellationToken = default
        )
        {
            var query = AddIncludes(_dbSet);
            return await query.FirstOrDefaultAsync(
                e => EF.Property<TId>(e, _primaryKeyName)!.Equals(id),
                cancellationToken
            );
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = AddIncludes(_dbSet).AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity> AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        )
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        )
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(
            TId id,
            CancellationToken cancellationToken = default
        )
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public virtual IQueryable<TEntity> GetQueryable() => AddIncludes(_dbSet);
    }
}
