using System.Collections.Generic;

namespace Doddle.Audit.Tests.Fakes
{
    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }

}
