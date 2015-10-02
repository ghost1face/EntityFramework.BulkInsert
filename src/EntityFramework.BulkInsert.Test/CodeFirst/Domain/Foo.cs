using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.BulkInsert.Test.CodeFirst.Domain
{
    public class Foo
    {
        public int Id { get; set; }
        public string Bar { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }

    public class FooExtended : Foo
    {
        public string ExtendedValue { get; set; }
    }
}
