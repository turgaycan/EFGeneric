using System;
using System.Linq;
using System.Linq.Expressions;
using EFGeneric.Base.Entity;
using EFGeneric.Base.Util;

namespace EFGeneric.Base.Repository {
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface IRepository<TEntity, in TId>
        where TEntity : BaseEntity, IEntity<TId>
        where TId : IComparable {

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        PaginatedList<TEntity> Paginate(int pageIndex, int pageSize);

        TEntity GetSingle(TId id);
    }
}