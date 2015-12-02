using System;
using System.Data.Entity;
using EFGeneric.Base.Entity;
using EFGeneric.Base.Entity.Core;

namespace EFGeneric.Context
{
    /// <summary>
    /// Store Custom Entity Context
    /// </summary>
    public interface IEntityContext<PK> : IDisposable
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity<PK>;
        void SetAsAdded<TEntity>(TEntity entity) where TEntity : BaseEntity<PK>;
        void SetAsModified<TEntity>(TEntity entity) where TEntity : BaseEntity<PK>;
        void SetAsDeleted<TEntity>(TEntity entity) where TEntity : BaseEntity<PK>;
        int Commit();
    }
}
