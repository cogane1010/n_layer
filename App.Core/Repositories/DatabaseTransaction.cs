using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Entity;

namespace App.Core.Repositories
{
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }

    public class DatabaseTransaction : IDatabaseTransaction
    {
        private IDbContextTransaction _transaction;

        public DatabaseTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}
