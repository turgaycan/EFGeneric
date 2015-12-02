using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EFGeneric.Base.Repository
{

    public interface IBaseRepository<T, PK> : IDisposable
    {
        IQueryable<T> All { get; }

        IQueryable<T> CurrentSession { get; }

        void Upsert(T entity, Func<T, bool> insertExpression);

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        T Find(params object[] keyValues);

        IQueryable<T> GetAll();

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Delete(T entity);

        void SoftDelete(T entity);

        void Update(T entity, bool nonsecure = true);

        T Merge(T entity);

        T FindById(PK id);

        IQueryable<T> FindByIds(List<T> entities);

    }
}
