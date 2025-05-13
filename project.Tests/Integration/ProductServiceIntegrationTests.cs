using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Data.Entities;
using project.Data.Repositories;
using project.Service.DTOs;
using project.Service.Services;
using project.Service.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace project.Tests.Integration
{
    public class ProductServiceIntegrationTests
    {
        private DbContextOptions<ProjectContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<ProjectContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
                cfg.AddProfile<CategoryProfile>();
            });
            return config.CreateMapper();
        }

        [Fact]
        public void AddProduct_ValidInput_PersistsToDatabase()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            var category = new Category { Name = "Electronics" };
            context.Categories.Add(category);
            context.SaveChanges();

            var categoryRepo = new CategoryRepository(context);
            var productRepo = new ProductRepository(context);
            var mapper = GetMapper();

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var dto = new ProductDTO
            {
                Name = "Laptop",
                Description = "Laptop desc",
                Price = 100,
                Quantity = 5,
                Categories = new List<string> { "Electronics" }
            };

            var result = service.AddProduct(dto);

            var saved = context.Products.Include(p => p.Categories).FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Laptop", saved.Name);
            Assert.Contains(saved.Categories, c => c.Name == "Electronics");
        }

        [Fact]
        public void AddProduct_WithNonexistentCategory_ThrowsException()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);

            var categoryRepo = new CategoryRepository(context);
            var productRepo = new ProductRepository(context);
            var mapper = GetMapper();

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var dto = new ProductDTO
            {
                Name = "Mouse",
                Description = "Mouse desc",
                Price = 20,
                Quantity = 2,
                Categories = new List<string> { "Doesn't exist" }
            };

            var ex = Assert.Throws<InvalidOperationException>(() => service.AddProduct(dto));
            Assert.Equal("Category 'Doesn't exist' does not exist.", ex.Message);
        }

        [Fact]
        public void StockImport_AddsAndUpdatesProductsCorrectly()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            var categoryRepo = new CategoryRepository(context);
            var productRepo = new ProductRepository(context);
            var mapper = GetMapper();
            var service = new ProductService(productRepo, categoryRepo, mapper);

            var stockList = new List<StockDTO>
            {
                new StockDTO { Name = "Monitor", Price = 200, Quantity = 3, Categories = new List<string> { "Monitors" } },
                new StockDTO { Name = "Monitor", Price = 200, Quantity = 2, Categories = new List<string> { "Monitors" } }
            };

            service.StockImport(stockList);

            var product = context.Products.Include(p => p.Categories).FirstOrDefault(p => p.Name == "Monitor");
            Assert.NotNull(product);
            Assert.Equal(5, product.Quantity);
            Assert.Single(product.Categories);
            Assert.Equal("monitors", product.Categories.First().Name);
        }

        [Fact]
        public void CalculateDiscount_ValidCart_ReturnsCorrectTotals()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);

            var category = new Category { Name = "Keyboards" };
            var product = new Product
            {
                Name = "Keyboard",
                Price = 100,
                Quantity = 10,
                Categories = new List<Category> { category }
            };

            context.Categories.Add(category);
            context.Products.Add(product);
            context.SaveChanges();

            var categoryRepo = new CategoryRepository(context);
            var productRepo = new ProductRepository(context);
            var mapper = GetMapper();
            var service = new ProductService(productRepo, categoryRepo, mapper);

            var cart = new List<CartItemDTO>
            {
                new CartItemDTO { ProductName = "Keyboard", Quantity = 2 }
            };

            var result = service.CalculateDiscount(cart);

            Assert.Equal(200, result.TotalBeforeDiscount);
            Assert.Equal(5, result.TotalDiscount); 
            Assert.Equal(195, result.FinalPrice);
            Assert.Single(result.Discounts);
            Assert.Equal("Keyboard", result.Discounts[0].ProductName);
            Assert.Equal(5, result.Discounts[0].DiscountAmount);
        }
    }
}
