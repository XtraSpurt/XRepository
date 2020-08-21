using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace XtraSpurt.XRepository
{
    public interface IXRepositoryBase<TEntity> where TEntity : class
    {
        Task ContextSaveAsync(CancellationToken cancellationToken = default);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string[] includeProperties = null,
            bool asNoTracking = false, CancellationToken cancellationToken = default);

        IQueryable<TEntity> GetIQueryable(bool asNoTracking = false, string[] includeProperties = null);

        Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false,
            string[] includeProperties = null, CancellationToken cancellationToken = default);

        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entityToUpdate);
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> filter, int batchSize = 1000, CancellationToken cancellationToken = default);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false,
            string[] includeProperties = null, CancellationToken cancellationToken = default);

        Task InsertRangeAsync(List<TEntity> listEntity, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetOrderedListAsync(Expression<Func<TEntity, bool>> filter, string order,
            bool asNoTracking = false, string[] includeProperties = null, CancellationToken cancellationToken = default);

        Task LoadCollectionAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> expression, CancellationToken cancellationToken = default) where TProperty : class;

        Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expression, CancellationToken cancellationToken = default)
            where TProperty : class;

        Task<TO> UsingTransactionAsync<TO>(Func<TO> function,CancellationToken cancellationToken = default);
        Task<TO> UsingTransactionAsync<TO>(Func<Task<TO>> function, CancellationToken cancellationToken = default);

        Task UsingTransactionAsync(Action function, CancellationToken cancellationToken = default);
    }
}

