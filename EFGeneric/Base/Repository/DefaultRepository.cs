using System;
using EFGeneric.Base.Entity;

namespace EFGeneric.Base.Repository {
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity">long</typeparam>
    public interface DefaultRepository<TEntity> : IDisposable, IRepository<TEntity, long>
        where TEntity : BaseEntity, IEntity<long>
    {
    }
}