using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Test.Domain
{
    public abstract class EmployeeTPT
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
    }

    public class WorkerTPT : EmployeeTPT
    {

        public virtual ManagerTPT Boss { get; set; }
    }

    public class ManagerTPT : EmployeeTPT
    {
        public string Rank { get; set; }
        public virtual ICollection<WorkerTPT> Henchmen { get; set; } 
    }


    public abstract class EmployeeTPH
    {
        public string NameWithTitle { get { return string.Format("{0} ({1})", Name, Title); } }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }

    public class AWorkerTPH : EmployeeTPH
    {
        public int BossId { get; set; }
        public int RefId { get; set; }
        public virtual ManagerTPH Boss { get; set; }
    }

    public class ManagerTPH : EmployeeTPH
    {
        public string Rank { get; set; }
        public int? RefId { get; set; }
        public virtual ICollection<AWorkerTPH> Henchmen { get; set; } 
    }
}
