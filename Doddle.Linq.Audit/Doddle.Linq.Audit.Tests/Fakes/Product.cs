using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;

namespace Doddle.Linq.Audit.Tests.Fakes
{
    [AuditResolver(typeof(ProductAuditResolver))]
    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        

        public int CategoryID { get; set; }
    }

}
