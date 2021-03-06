﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GenRepo.Client.Model;
using NUnit.Framework;

namespace GenRepo.Client
{
    [TestFixture]
    public class RepositoryUsage
    {
        private IRepository _repository;
        private AssemblyDbContext _scalableDbContext;

        [SetUp]
        public void Setup()
        {
            _scalableDbContext = new AssemblyDbContext(Assembly.GetExecutingAssembly(), "EFDataAccess");
            _repository = new GenericRepository(_scalableDbContext);
            AddDataToRepository();
        }

        [TearDown]
        public void Teardown()
        {
            _scalableDbContext.Database.Delete();
        }

        [Test]
        public void ProjectionTest()
        {
            var q = Query.WithFilter(Filter<Product>.Create(p => p.Price > 30)).ToProjection(ReusableProjections.ProductView).OrderBy(o=>o.Asc(p=>p.BrandName));

            var items = _repository.Get(q).ToArray();

            Assert.That(items.Length,Is.EqualTo(3));
        }

        private void AddDataToRepository()
        {
            var dell = new Brand() { Name = BrandName.Dell };

            var samsung = new Brand() { Name = BrandName.Samsung };

            var product = new Product() { Name = "Mini keyboard", Category = ProductCategory.KeyBoard, AverageRatings = 5, Price = 30, Brand = samsung, CreatedOn = DateTime.Now };
            _repository.Add(product);
            product = new Product() { Name = "keyboard", Category = ProductCategory.KeyBoard, AverageRatings = 3, Price = 50, Brand = samsung, CreatedOn = DateTime.UtcNow };
            _repository.Add(product);

            product = new Product() { Name = "Mini keyboard", Category = ProductCategory.KeyBoard, AverageRatings = 5, Price = 31, Brand = dell, CreatedOn = DateTime.UtcNow };
            _repository.Add(product);
            product = new Product() { Name = "keyboard", Category = ProductCategory.KeyBoard, AverageRatings = 5, Price = 60, Brand = dell, CreatedOn = DateTime.UtcNow };
            _repository.Add(product);
            product = new Product() { Name = "Good mouse", Category = ProductCategory.Mouse, AverageRatings = 5, Price = 10, Brand = samsung, CreatedOn = DateTime.UtcNow };
            _repository.Add(product);

            _repository.Save();
        }

        private class ReusableProjections
        {
            public static Expression<Func<Product, ProductView>> ProductView 
                => p => new ProductView() { BrandName = p.Brand.Name, Id = p.Id, Name = p.Name };
        }

        private class ProductView
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string BrandName { get; set; }
        }
         
    }
}