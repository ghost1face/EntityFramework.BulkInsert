using System;

namespace EntityFramework.BulkInsert.Test.Domain
{
    public interface ICreatedAt
    {
        DateTime CreatedAt { get; set; }
    }
}