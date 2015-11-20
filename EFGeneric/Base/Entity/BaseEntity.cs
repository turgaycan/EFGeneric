using System;

namespace EFGeneric.Base.Entity
{
    public abstract class BaseEntity<TId> where TId : IComparable
    {
        public virtual TId Id { get; set; }
    }
}
