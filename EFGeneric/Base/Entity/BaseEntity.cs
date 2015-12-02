using System.ComponentModel.DataAnnotations;
using EFGeneric.Base.Entity.Core;

namespace EFGeneric.Base.Entity
{
    public abstract class BaseEntity<PK> : IEntity<PK>
    {
        [Key]
        public virtual PK Id { get; set; }

        public bool IsPersisted()
        {
            if (Id is int)
            {
                return (int)(object)Id > 0;
            }
            if (Id is long)
            {
                return (long)(object)Id > 0;
            }
            return Id != null;
        }

        public bool IsNotPersisted()
        {
            return !IsPersisted();
        }
    }
}
