﻿namespace Doddle.Audit.Tests.Fakes
{
    [AuditResolver(typeof(ProductAuditResolver))]
    public class Product
    {
        public int ID { get; set; }
        public string ProductName { get; set; }
        

        public int CategoryID { get; set; }
    }

}
