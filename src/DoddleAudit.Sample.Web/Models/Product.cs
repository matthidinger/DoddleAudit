using System.Collections.Generic;

namespace DoddleAudit.Sample.Web.Models
{
    public class Product
    {
        public Product()
        {
            Promotions = new HashSet<Promotion>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }

        public virtual int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }
    }
}