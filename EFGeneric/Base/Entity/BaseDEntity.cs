using System;
using EFGeneric.Base.Entity.Core;

namespace EFGeneric.Base.Entity
{
    public class BaseDEntity<PK> : BaseEntity<PK>, IDeletable
    {
        public virtual Nullable<bool> IsDeleted { get; set; }

        public bool IsSoftDeleted()
        {
            return IsDeleted != null && IsDeleted == true;
        }
    }
}
