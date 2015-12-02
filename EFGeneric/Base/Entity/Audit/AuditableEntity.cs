using System;
using System.ComponentModel.DataAnnotations;

namespace EFGeneric.Base.Entity.Audit
{
    public abstract class AuditableEntity<T, PK> : BaseEntity<PK>, IAuditableEntity
    {
        [ScaffoldColumn(false)]
        public DateTime CreatedDate { get; set; }

        [MaxLength(100)]
        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime UpdatedDate { get; set; }

        [MaxLength(100)]
        [ScaffoldColumn(false)]
        public string UpdatedBy { get; set; }
    }
}
