using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.BulkInsert.Test
{
    /// <summary>
    /// Child of abstract parent.
    /// </summary>
    [Table("ApChilds")]
    public class ApChild : AbstractParent
    {
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
