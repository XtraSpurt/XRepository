using System;
using System.Threading;
using System.Threading.Tasks;

namespace XtraSpurt.XRepository
{
    public interface IXUnitOfWork
    {
        /// <summary>
        ///     Commits all changes
        /// </summary>
        void SaveChanges();


        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///     Discards all changes that has not been commited
        /// </summary>
        void RejectChanges();


        void Dispose();

        Task<TO> UsingTransaction<TO>(Func<TO> function, CancellationToken cancellationToken = default);


        Task UsingTransaction(Action function, CancellationToken cancellationToken = default);
    }
}