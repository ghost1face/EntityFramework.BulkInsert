using System;

namespace DataAccess
{
    public class IntervalEntity : XafGcEntity, IIntervalEntity<Guid>
    {
        public Guid ResourceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public interface IIntervalEntity<T> : IResourceEntity<T>
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
    }

    public interface IResourceEntity<T> : IEntity
    {
        T ResourceId { get; set; }
    }

    public interface IEntity
    {
        Guid Oid { get; set; }

        int? GcRecord { get; set; }
    }
}