using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using EFGeneric.Base.Entity;
using EFGeneric.Base.Entity.Audit;

namespace EFGeneric.Context
{
    public abstract class EntityContext<PK> : DbContext, IEntityContext<PK>
    {

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of
        /// the database to which a connection will be made. The by-convention name is
        /// the full name (namespace + class name) of the derived context class.  See
        /// the class remarks for how this is used to create a connection. 
        /// </summary>
        protected EntityContext()
            : base()
        {
        }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of
        /// the database to which a connection will be made, and initializes it from
        /// the given model.  The by-convention name is the full name (namespace + class
        /// name) of the derived context class.  See the class remarks for how this is
        /// used to create a connection.
        /// </summary>
        /// <param name="model">The model that will back this context.</param>
        protected EntityContext(DbCompiledModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection
        /// string for the database to which a connection will be made.  See the class
        /// remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public EntityContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect
        /// to a database.  The connection will not be disposed when the context is disposed.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">
        /// If set to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </param>
        public EntityContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Constructs a new context instance around an existing ObjectContext.  An existing
        /// ObjectContext to wrap with the new context.  If set to true the ObjectContext
        /// is disposed when the EntityContext is disposed, otherwise the caller must dispose
        /// the connection.
        /// </summary>
        /// <param name="objectContext">An existing ObjectContext to wrap with the new context.</param>
        /// <param name="EntityContextOwnsObjectContext">
        /// If set to true the ObjectContext is disposed when the EntityContext is disposed,
        /// otherwise the caller must dispose the connection.
        /// </param>
        public EntityContext(ObjectContext objectContext, bool EntityContextOwnsObjectContext)
            : base(objectContext, EntityContextOwnsObjectContext)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection
        /// string for the database to which a connection will be made, and initializes
        /// it from the given model.  See the class remarks for how this is used to create
        /// a connection.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="model">The model that will back this context.</param>
        public EntityContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect
        /// to a database, and initializes it from the given model.  The connection will
        /// not be disposed when the context is disposed.  An existing connection to
        /// use for the new context.  The model that will back this context.  If set
        /// to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="model">The model that will back this context.</param>
        /// <param name="contextOwnsConnection">
        /// If set to true the connection is disposed when the context is disposed, otherwise
        /// the caller must dispose the connection.
        /// </param>
        public EntityContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
        }

        /// <summary>
        /// Traditional way of execute SQL Command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        public void ExecuteCommand(string command, params object[] parameters)
        {
            Database.ExecuteSqlCommand(command, parameters);
        }

        public new IDbSet<T> Set<T>() where T : BaseEntity<PK>
        {
            return base.Set<T>();
        }

        public void SetAsAdded<T>(T entity) where T : BaseEntity<PK>
        {
            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Added;
        }

        public void SetAsModified<T>(T entity) where T : BaseEntity<PK>
        {
            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public void SetAsDeleted<T>(T entity) where T : BaseEntity<PK>
        {
            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public int Commit()
        {
            return SaveChanges();
        }

        /// <summary>
        /// CRUD işlemlerinde Commit() metodu çağrılmaktadır.. 
        /// Commit metodu ile birlikte Audit sınıflar için çalışır!
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditableEntity
                    && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                IAuditableEntity entity = entry.Entity as IAuditableEntity;
                if (entity == null)
                {
                    continue;
                }

                string identityName = Thread.CurrentPrincipal.Identity.Name;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedBy = identityName;
                    entity.CreatedDate = now;
                }
                else
                {
                    Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedBy = identityName;
                entity.UpdatedDate = now;
            }

            return base.SaveChanges();
        }

        private DbEntityEntry GetDbEntityEntrySafely<T>(T entity) where T : BaseEntity<PK>
        {

            DbEntityEntry dbEntityEntry = Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                Set<T>().Attach(entity);
            }

            return dbEntityEntry;
        }

    }
}
