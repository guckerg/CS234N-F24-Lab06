using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        MMABooksContext dbContext;
        Product? p;
        List<Product>? products;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            products = dbContext.Products.OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("A4CS", products[0].ProductCode);
            PrintAll(products);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            p = dbContext.Products.Find("A4CS");
            Assert.IsNotNull(p);
            Assert.AreEqual("Murach's ASP.NET 4 Web Programming with C# 2010", p.Description);
            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50
            products = dbContext.Products.Where(p => p.UnitPrice == 56.50m).OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(7, products.Count);
            PrintAll(products);
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            // get a list of objects that include the productcode, unitprice, quantity and inventoryvalue
            var products = dbContext.Products.Select(
            p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity }).
            OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            p = dbContext.Products.Find("ZJLR");
            dbContext.Products.Remove(p);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Products.Find("ZJLR"));
        }

        [Test]
        public void CreateTest()
        {
            p = new Product();
            p.ProductCode = "XXXX";
            p.Description = "Test Description";
            p.UnitPrice = 56.75m;
            p.OnHandQuantity = 794;
            dbContext.Products.Add(p);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Products.Find("XXXX"));
        }

        [Test]
        public void UpdateTest()
        {
            p = dbContext.Products.Find("A4CS");
            p.Description = "test update description";
            dbContext.Products.Update(p);
            dbContext.SaveChanges();
            dbContext.Products.Find("A4CS");
            Assert.AreEqual("test update description", p.Description);
        }

        public void PrintAll(List<Product> products)
        {
            foreach (Product p in products)
            {
                Console.WriteLine(p);
            }
        }
    }
}