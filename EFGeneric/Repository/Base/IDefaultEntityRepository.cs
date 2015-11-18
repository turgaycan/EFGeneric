using System;
using EFGeneric.Base.Entity;

namespace EFGeneric.Repository.Base {

    /// <summary>
    /// Entity Framework interface implementation for IRepository.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity which implements IEntity of long</typeparam>
    public interface IDefaultEntityRepository<TEntity> : IEntityRepository<TEntity, long>
        where TEntity : BaseEntity, IEntity<long>
    {

    }
}