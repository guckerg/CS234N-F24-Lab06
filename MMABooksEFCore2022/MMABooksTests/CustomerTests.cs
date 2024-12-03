using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers.OrderBy(c => c.CustomerId).ToList();
            Assert.AreEqual(696, customers.Count);
            Assert.AreEqual(1, customers[0].CustomerId);
            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(3);
            Assert.IsNotNull(c);
            Assert.AreEqual("Antony, Abdul", c.Name);
            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            customers = dbContext.Customers.Where(c => c.Name.StartsWith("A")).OrderBy(c => c.Name).ToList();
            Assert.AreEqual(37, customers.Count);
            Assert.AreEqual("Abeyatunge, Derek", customers[0].Name);
            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            c = dbContext.Customers.Include("Invoices").Where(c => c.CustomerId == 20).SingleOrDefault();
            Assert.IsNotNull(c);
            Assert.AreEqual(20, c.CustomerId);
            Assert.AreEqual(3, c.Invoices.Count);
            Console.WriteLine(c);
        }

        /*
        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.StateCode,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.StateCode, s.StateName }).OrderBy(r => r.StateName).ToList();
            Assert.AreEqual(696, customers.Count);
            // I wouldn't normally print here but this lets you see what each object looks like
            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }*/

        [Test]
        public void DeleteTest()
        {
            c = dbContext.Customers.Find(1);
            dbContext.Customers.Remove(c);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Customers.Find(1));
        }

        [Test]
        public void CreateTest()
        {
            c = new Customer();
            c.Name = "Gucker, Gabe";
            c.Address = "393 Lenore Loop";
            c.City = "Eugene";
            c.State = "OR";
            c.ZipCode = "97404";
            dbContext.Customers.Add(c);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Customers.Find(c.CustomerId));
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.Find(4);
            c.Name = "Gucker, Sarah";
            dbContext.Customers.Update(c);
            dbContext.SaveChanges();
            dbContext.Customers.Find(4);
            Assert.AreEqual("Sarah Gucker", c.Name);
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
    }
}