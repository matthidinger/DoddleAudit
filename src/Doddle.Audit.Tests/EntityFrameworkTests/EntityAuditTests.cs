//using System;
//using System.Reflection;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Doddle.Linq.Audit.Tests.EntityFrameworkTests;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Rhino.Mocks;

//namespace Doddle.Linq.Audit.Tests
//{
//    [TestClass]
//    public class EntityAuditTests
//    {
//        [TestMethod]
//        public void AuditableObjectContext_Audits_Correctly()
//        {
//            NorthwindEntities context = GetContext();
//            //context.Audit<Product>(p => p.ProductID);
//            context.Audit<Category>().AuditAssociation<Product>(p => p.ProductID, p => p.Categories.CategoryID);
       
//            Assert.AreEqual(1, context.AuditDefinitions.Count);


            
//            Category category = context.CategorySet.First(c => c.CategoryID == 5);
            
//            Product product = new Product();
//            product.ProductName = "test20";
//            product.Categories = category;
//            //context.AddToProductSet(product);


//            //Product edited = context.ProductSet.First(p => p.ProductID == 1);
//            //edited.ProductName = "Edited product";
//            //edited.Categories = category;
            

//            //Product deleted = context.ProductSet.Include("Categories").First(p => p.ProductID == 110);
//            //context.DeleteObject(deleted);

//            context.SaveChanges();

//            int productId = product.ProductID;
//        }

//        private NorthwindEntities GetContext()
//        {
//            return new NorthwindEntities();
//        }
//    }
//}
