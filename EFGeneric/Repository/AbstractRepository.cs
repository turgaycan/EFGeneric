using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EFGeneric.Base.Entity;
using EFGeneric.Base.Repository;
using EFGeneric.Base.Util;
using EFGeneric.Context;

namespace EFGeneric.Repository
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="C">Db/Object Context</typeparam>
    /// <typeparam name="T">Entity</typeparam>
    public abstract class AbstractRepository<C, T, PK> : IBaseRepository<T, PK>
        where C : EntityContext<PK>, new()
        where T : BaseEntity<PK>
    {
        private C _entities = Activator.CreateInstance<C>();
        private bool disposed;

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

        public virtual IQueryable<T> GetAll()
        {
            return _entities.Set<T>();
        }


        public virtual IQueryable<T> CurrentSession
        {
            get
            {
                return GetCurrentSession();
            }
        }

        protected IQueryable<T> GetCurrentSession()
        {
            var query = GetAll();
            var isDeletedExists = typeof(T).GetProperty("IsDeleted");

            if (isDeletedExists != null)
            {
                query = query.Where(PredicateExtensions.EqualOrAddNullValueCondition<T>("IsDeleted", false));
            }

            return query;
        }

        

        /// <summary>
        /// IMPORTANT!! USE ONLY ENTITIES DERIVED FROM BASEENTITY<T>
        /// </summary>
        /// <param name="id">Entity that is derived from BaseEntity<object></object></param>
        /// <returns></returns>
        public virtual T FindById(PK id)
        {
            return GetCurrentSession().FirstOrDefault(PredicateExtensions.Equal<T>("Id", id));
        }

        public IQueryable<T> FindByIds(List<T> entities)
        {
            //use for this extension utils..
            throw new NotImplementedException();
        }


        /// <summary>
        /// //do implementation sub class!
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual T Merge(T entity)
        {

            if (entity == null)
            {
                throw new ObjectNotFoundException();
            }

            if (entity.IsNotPersisted())
            {
                Add(entity);
                return entity;
            }

            T t = FindById(entity.Id);
            if (t == null)
            {
                throw new ObjectNotFoundException();
            }

            _entities.Entry(t).State = EntityState.Detached;

            Commit();

            return t;
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = (IQueryable<T>)_entities.Set<T>();
            return queryable;
            //return includeProperties.Aggregate(queryable, (current, expression) => (IQueryable<T>)QueryableExtensions.Include<T, object>((IQueryable<T>)current,
            //    (Expression<Func<T, T>>)expression));
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
            Commit();
        }

        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
            Commit();
        }

        public void SoftDelete(T entity)
        {
            if (entity.IsPersisted() && entity.GetType().GetProperty("IsDeleted") != null)
            {
                entity.GetType().GetProperty("IsDeleted").SetValue(entity, true);
                Update(entity);
            }
            else
            {
                throw new InvalidOperationException("This entity type does not support soft deletion." +
                                                    " Please add a bool? property called IsDeleted and try again.");
            }

        }

        public virtual void Update(T entity, bool nonsecure = true)
        {
            if (!nonsecure)
            {
                entity = _entities.Set<T>().Find(entity);
                if (entity == null)
                    throw new ObjectNotFoundException();

            }
            _entities.Entry(entity).State = EntityState.Detached;

            Commit();
        }

        public virtual void Upsert(T entity, Func<T, bool> insertExpression)
        {
            if (insertExpression(entity))
                Add(entity);
            else
                Update(entity);

            Commit();
        }

        public virtual void Commit()
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
