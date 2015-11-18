using System;
using System.Linq;
using System.Linq.Expressions;
using EFGeneric.Base.Entity;

namespace EFGeneric.Base.Repository
{

    public interface IBaseRepository<T> : IDisposable where T : BaseEntity
    {
        IQueryable<T> All { get; }

        void Upsert(T entity, Func<T, bool> insertExpression);

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        T Find(params object[] keyValues);

        IQueryable<T> GetAll();

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        void Add(T entity);

        void Delete(T entity);

        void Edit(T entity);

        void Save();
    }
}
