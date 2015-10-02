using System;

namespace EntityFramework.BulkInsert.Test.Domain
{
    public interface IModifiedAt
    {
        DateTime? ModifiedAt { get; set; }
    }
}