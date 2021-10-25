using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.BulkInsert.Test
{
    [Table("RenamedDocument")]
    public class RenamedParent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [MaxLength(10)]
        public string Number { get; set; }

        public bool IsEmpty { get; set; }
    }
}
