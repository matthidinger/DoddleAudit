using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doddle.Linq.Audit.Tests.Fakes
{
    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

}
