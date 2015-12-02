
using System;

namespace EFGeneric.Base.Entity.Core
{
    public interface IDeletable
    {
        Nullable<bool> IsDeleted { get; set; }
    }
}
