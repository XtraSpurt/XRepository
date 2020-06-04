using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace XtraSpurt.XRepository
{
    public interface IXRepositoryBase<TEntity> where TEntity : class
    {
        Task ContextSaveAsync();
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string[] includeProperties = null,
            bool asNoTracking = false);

        IQueryable<TEntity> GetIQueryable(bool asNoTracking = false, string[] includeProperties = null);

        Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false,
            string[] includeProperties = null);

        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        void Update(TEntity entityToUpdate);
        Task<int> GetCountAsync();
        Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> filter, int batchSize = 1000);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, bool asNoTracking = false,
            string[] includeProperties = null);

        Task InsertRangeAsync(List<TEntity> listEntity);

        Task<List<TEntity>> GetOrderedList(Expression<Func<TEntity, bool>> filter, string order,
            bool asNoTracking = false, string[] includeProperties = null);

        Task LoadCollectionAsync<TProperty>(TEntity entity,
            Expression<Func<TEntity, IEnumerable<TProperty>>> expression) where TProperty : class;

        Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expression)
            where TProperty : class;

        Task<TO> UsingTransaction<TO>(Func<TO> function);

        Task UsingTransaction(Action function);
    }
}

