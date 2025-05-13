using AutoMapper;
using Moq;
using project.Data.Entities;
using project.Data.Interfaces;
using project.Service.DTOs;
using project.Service.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace project.Tests.Services
{
    public class ProductServiceTests
    {
        IProductRepository productRepo;
        ICategoryRepository categoryRepo;
        IMapper mapper;

        Mock<IProductRepository> productRepositoryMock = new();
        Mock<ICategoryRepository> categoryRepositoryMock = new();
        Mock<IMapper> mapperMock = new();

        Product product;
        ProductDTO productDTO;
        List<Product> productList = new();
        List<ProductDTO> productDTOList = new();
        List<Category> categoryList = new();

        private Product GetProduct()
        {
            return new Product
            {
                Id = 1,
                Name = "Keyboard",
                Description = "Keyboard desc",
                Price = 66,
                Quantity = 5,
                Categories = new List<Category> { new Category { Id = 1, Name = "Keyboards" } }
            };
        }

        private ProductDTO GetProductDTO()
        {
            return new ProductDTO
            {
                Id = 1,
                Name = "Keyboard",
                Description = "Keyboard desc",
                Price = 66,
                Quantity = 5,
                Categories = new List<string> { "Keyboards" }
            };
        }

        private void SetupMocks()
        {
            productRepo = productRepositoryMock.Object;
            categoryRepo = categoryRepositoryMock.Object;
            mapper = mapperMock.Object;
        }

        [Fact]
        public void AddProduct_ValidProduct_ReturnsProductDTO()
        {
            SetupMocks();

            productDTO = GetProductDTO();
            product = GetProduct();
            categoryList = product.Categories.ToList();

            mapperMock.Setup(m => m.Map<Product>(productDTO)).Returns(product);
            mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDTO);
            productRepositoryMock.Setup(r => r.GetProducts()).Returns(new List<Product>());
            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(categoryList);

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var result = service.AddProduct(productDTO);

            Assert.NotNull(result);
            Assert.Equal("Keyboard", result.Name);
            productRepositoryMock.Verify(r => r.AddProduct(product), Times.Once);
        }

        [Fact]
        public void AddProduct_WithExistingName_ThrowsException()
        {
            SetupMocks();

            productDTO = GetProductDTO();
            product = GetProduct();

            mapperMock.Setup(m => m.Map<Product>(productDTO)).Returns(product);
            productRepositoryMock.Setup(r => r.GetProducts()).Returns(new List<Product> { product });

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var ex = Assert.Throws<InvalidOperationException>(() => service.AddProduct(productDTO));
            Assert.Equal("Product with that name already exists.", ex.Message);
        }

        [Fact]
        public void AddProduct_WithNegativePrice_ThrowsException()
        {
            SetupMocks();

            productDTO = GetProductDTO();
            productDTO.Price = -10;
            product = GetProduct();
            product.Price = -10;
            product.Name = "Keyboard";
            product.Categories = new List<Category> { new Category { Id = 1, Name = "Keyboards" } };

            mapperMock.Setup(m => m.Map<Product>(productDTO)).Returns(product);
            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(product.Categories);

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var ex = Assert.Throws<InvalidOperationException>(() => service.AddProduct(productDTO));
            Assert.Equal("Product price must be greater than 0.", ex.Message);
        }

        [Fact]
        public void AddProduct_WithMissingCategories_ThrowsException()
        {
            SetupMocks();

            productDTO = GetProductDTO();
            productDTO.Categories = null;
            product = GetProduct();
            product.Categories = null;

            mapperMock.Setup(m => m.Map<Product>(productDTO)).Returns(product);

            var service = new ProductService(productRepo, categoryRepo, mapper);

            var ex = Assert.Throws<InvalidOperationException>(() => service.AddProduct(productDTO));
            Assert.Equal("At least one category is required.", ex.Message);
        }

        [Fact]
        public void GetProductById_ValidId_ReturnsProductDTO()
        {
            SetupMocks();
            product = GetProduct();
            productDTO = GetProductDTO();

            productRepositoryMock.Setup(r => r.GetProductById(1)).Returns(product);
            mapperMock.Setup(m => m.Map<ProductDTO>(product)).Returns(productDTO);

            var service = new ProductService(productRepo, categoryRepo, mapper);
            var result = service.GetProductById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void DeleteProduct_ExistingProduct_ReturnsTrue()
        {
            SetupMocks();
            product = GetProduct();

            productRepositoryMock.Setup(r => r.GetProductById(1)).Returns(product);
            productRepositoryMock.Setup(r => r.DeleteProduct(product)).Returns(true);

            var service = new ProductService(productRepo, categoryRepo, mapper);
            var result = service.DeleteProduct(1);

            Assert.True(result);
        }

        [Fact]
        public void CalculateDiscount_ValidCart_ReturnsCorrectDiscount()
        {
            SetupMocks();
            var cart = new List<CartItemDTO>
            {
                new CartItemDTO { ProductName = "Keyboard", Quantity = 2 }
            };

            product = GetProduct();
            productRepositoryMock.Setup(r => r.GetProducts()).Returns(new List<Product> { product });

            var service = new ProductService(productRepo, categoryRepo, mapper);
            var result = service.CalculateDiscount(cart);

            Assert.Equal(132m, result.TotalBeforeDiscount);
            Assert.Equal(3.3m, result.TotalDiscount);
            Assert.Equal(128.7m, result.FinalPrice);
            Assert.Single(result.Discounts);
        }

        [Fact]
        public void StockImport_NewProduct_AddsSuccessfully()
        {
            SetupMocks();
            var stockList = new List<StockDTO>
            {
                new StockDTO
                {
                    Name = "Mouse",
                    Price = 25,
                    Quantity = 10,
                    Categories = new List<string> { "New category" }
                }
            };

            var existingProducts = new List<Product>();
            var existingCategories = new List<Category>();
            productRepositoryMock.Setup(r => r.GetProducts()).Returns(existingProducts);
            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(existingCategories);

            var service = new ProductService(productRepo, categoryRepo, mapper);
            service.StockImport(stockList);

            productRepositoryMock.Verify(r => r.AddProduct(It.Is<Product>(p => p.Name == "Mouse")), Times.Once);
        }
    }
}