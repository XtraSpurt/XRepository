using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace XtraSpurt.XRepository.Repository
{
    public class XRepositoryBase<TEntity> : IXRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly ILogger<XRepositoryBase<TEntity>> _logger;
        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;

        public XRepositoryBase(DbContext context, ILogger<XRepositoryBase<TEntity>> logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetIQueryable(bool asNoTracking = false, string[] includeProperties = null)
        {
            var result = (IQueryable<TEntity>) _dbSet;
            if (includeProperties != null)
                if (includeProperties.Any())
                    result = includeProperties.Aggregate(result, (current, includeProperty) => current.Include(includeProperty));
            if (asNoTracking) result = result.AsNoTracking();
            return result;
        }

        public virtual async Task<List<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string[] includeProperties = null, bool asNoTracking = false)
        {
            var query = GetIQueryable(asNoTracking, includeProperties);

            if (filter != null) query = query.Where(filter);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false)
        {
            return await GetIQueryable(asNoTracking).AnyAsync(filter);
        }

        public virtual async Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
            bool asNoTracking = false, string[] includeProperties = null)
        {
            return await GetIQueryable(asNoTracking, includeProperties).SingleOrDefaultAsync(filter);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            var r = _dbSet.Add(entity);
            return r.Entity;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            var r = await _dbSet.AddAsync(entity);
            return r.Entity;
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached) _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> filter, int batchSize = 1000)
        {
            return await _dbSet.Where(filter).DeleteAsync(x => x.BatchSize = batchSize);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class
        {
            await _context.Entry(entity).Collection(expression).LoadAsync();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, TProperty>> expression) where TProperty : class
        {
            await _context.Entry(entity).Reference(expression).LoadAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await GetIQueryable().CountAsync();
        }

        public virtual async Task ContextSaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
            bool asNoTracking = false, string[] includeProperties = null)
        {
            return await GetIQueryable(asNoTracking, includeProperties).FirstOrDefaultAsync(filter);
        }

        public virtual async Task InsertRangeAsync(List<TEntity> listEntity)
        {
            await _dbSet.AddRangeAsync(listEntity);
        }

        public async Task<List<TEntity>> GetOrderedList(Expression<Func<TEntity, bool>> filter, string order,
            bool asNoTracking = false, string[] includeProperties = null)
        {
            var query = GetIQueryable(asNoTracking, includeProperties);

            if (filter != null) query = query.Where(filter);

            if (!string.IsNullOrEmpty(order)) query = query.OrderByProperties(order);
            return await query.ToListAsync();
        }


        public async Task UsingTransaction(Action function)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                function();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Unable to Execute Database Transaction ");
                throw;
            }
        }


        public async Task<TO> UsingTransaction<TO>(Func<TO> function)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = function();
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                _logger.LogError(e, "Unable to Execute Database Transaction ");
                throw;
            }
        }
    }
}