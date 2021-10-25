using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.BulkInsert.Test
{
    [Table("Childs")]
    public class Child : Parent
    {
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
