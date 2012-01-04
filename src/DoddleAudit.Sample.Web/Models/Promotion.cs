namespace DoddleAudit.Sample.Web.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public decimal DiscountAmount { get; set; }

        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }

    }
}