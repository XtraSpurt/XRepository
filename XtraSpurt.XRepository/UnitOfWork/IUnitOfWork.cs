using System;
using System.Threading.Tasks;

namespace XtraSpurt.XRepository
{
    public interface IXUnitOfWork
    {
        /// <summary>
        ///     Commits all changes
        /// </summary>
        void SaveChanges();


        Task SaveChangesAsync();

        /// <summary>
        ///     Discards all changes that has not been commited
        /// </summary>
        void RejectChanges();


        void Dispose();

        Task<TO> UsingTransaction<TO>(Func<TO> function);


        Task UsingTransaction(Action function);
    }
}