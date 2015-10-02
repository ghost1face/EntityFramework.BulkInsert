using System;
using System.Collections.Generic;

namespace EntityFramework.BulkInsert.Test.Domain
{
    public class Page
    {
        
        public int PageId { get; set; }

        public string Content { get; set; }

        public string Title { get; set; }

        public int? ParentId { get; set; }

        public virtual Page Parent { get; set; }

        public virtual ICollection<PageTranslations> Translations { get; set; } 

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}