
using System;

namespace EFGeneric.Base.Entity {
    
    /// <summary>
    /// Default entity, parameterized by long (db type -> bigint)
    /// </summary>
    public abstract class BaseEntity : IEntity<long> {
        public virtual long Id { get; set; }
    }
}