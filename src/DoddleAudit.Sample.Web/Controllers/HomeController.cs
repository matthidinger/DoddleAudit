using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using DoddleAudit.Sample.Web.Models;

namespace DoddleAudit.Sample.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new ECommerceDb();

            var products = db.Products.ToList();

            using (var transaction = new TransactionScope())
            {
                var product = db.Products.Create();
                product.CategoryId = 1;
                product.ProductName = "Taco";

                product.Promotions.Add(new Promotion{ DiscountAmount = 5});

                db.Products.Add(product);
                db.SaveChanges();
            }

            return View();
        }
    }
}
