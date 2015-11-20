using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EFGeneric.Base.Repository;

namespace EFGeneric.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="C">Db/Object Context</typeparam>
    /// <typeparam name="T">Entity</typeparam>
    /// <typeparam name="TId">Id (BigInt, Int)</typeparam>
    public abstract class AbstractRepository<C, T> : IBaseRepository<T>, IDisposable
        where C : DbContext, new()
        where T : class
    {
        private C _entities = Activator.CreateInstance<C>();
        private bool disposed = false;

        protected C Context
        {
            get
            {
                return _entities;
            }
            set
            {
                _entities = value;
            }
        }

        public virtual IQueryable<T> All
        {
            get
            {
                return GetAll();
            }
        }

        public virtual T FindById(object id)
        {
            return GetAll().SingleOrDefault(entity => entity.Equals(id));
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = (IQueryable<T>)_entities.Set<T>();
            return queryable;
            //return includeProperties.Aggregate(queryable, (current, expression) => (IQueryable<T>)QueryableExtensions.Include<T, object>((IQueryable<T>)current,
            //    (Expression<Func<T, T>>)expression));
        }

        public virtual IQueryable<T> GetAll()
        {
            return _entities.Set<T>();
        }

        public virtual T Find(params object[] keyValues)
        {
            return _entities.Set<T>().Find(keyValues);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _entities.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            _entities.Set<T>().Add(entity);
            Save();
        }

        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
            Save();
        }

        public virtual void Edit(T entity)
        {
            //T t = _entities.Set<T>().AsNoTracking().FirstOrDefault(en => en.Id.Equals(entity.Id));
            //if (t == null)
            //    throw new ObjectNotFoundException();

            //_entities.Entry(t).State = EntityState.Detached;
            //Save();
        }

        public virtual void Upsert(T entity, Func<T, bool> insertExpression)
        {
            if (insertExpression(entity))
                Add(entity);
            else
                Edit(entity);

            Save();
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                _entities.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((object)this);
        }
    }

}
