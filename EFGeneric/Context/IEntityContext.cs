using System;
using System.Data.Entity;

namespace EFGeneric.Context
{
    /// <summary>
    /// Store Custom Entity Context
    /// </summary>
    public interface IEntityContext : IDisposable
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetAsAdded<TEntity>(TEntity entity) where TEntity : class;
        void SetAsModified<TEntity>(TEntity entity) where TEntity : class;
        void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
    }
}
