using System;
using System.Linq;
using System.Linq.Expressions;

namespace EFGeneric.Base.Repository
{

    public interface IBaseRepository<T> : IDisposable
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
