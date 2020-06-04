using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace XtraSpurt.XRepository
{
    public abstract class XUnitOfWork : IXUnitOfWork, IDisposable
    {
        protected readonly DbContext _context;
        protected readonly ILogger<XUnitOfWork> Logger;

        protected XUnitOfWork(DbContext context, ILogger<XUnitOfWork> logger)
        {
            _context = context;
            Logger = logger;
        }

        /// <summary>
        ///     Commits all changes
        /// </summary>
        public void SaveChanges()
        {
            _context.SaveChanges();
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        ///     Discards all changes that has not been commited
        /// </summary>
        public void RejectChanges()
        {
            foreach (var entry in _context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged))
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() => _context.Dispose();


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
                Logger.LogError(e, "Unable to Execute Database Transaction ");
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
                Logger.LogError(e, "Unable to Execute Database Transaction ");
                throw;
            }
        }
    }
}