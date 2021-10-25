using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.BulkInsert.Test
{
    [Table("RenamedChild")]
    public class RenamedChild : RenamedParent
    {
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
