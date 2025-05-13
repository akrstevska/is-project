using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.Data.Entities;
using project.Data.Interfaces;
using project.Service.DTOs;
using project.Service.Interfaces;

namespace project.Service.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository,
                              ICategoryRepository categoryRepository,
                              IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public ProductDTO AddProduct(ProductDTO productDTO)
        {
            var newProduct = _mapper.Map<Product>(productDTO);

            if (string.IsNullOrWhiteSpace(newProduct?.Name))
                throw new InvalidOperationException("Product name is required.");

            if (productDTO.Categories == null || !productDTO.Categories.Any())
                throw new InvalidOperationException("At least one category is required.");

            if (productDTO.Price <= 0)
                throw new InvalidOperationException("Product price must be greater than 0.");

            var existingProduct = _productRepository.GetProducts()
                .FirstOrDefault(p => p.Name.ToLower() == newProduct.Name.ToLower());

            if (existingProduct != null)
                throw new InvalidOperationException("Product with that name already exists.");

            var allCategories = _categoryRepository.GetCategories();
            var matchedCategories = new List<Category>();
            foreach (var name in productDTO.Categories)
            {
                var category = allCategories
                    .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (category == null)
                    throw new InvalidOperationException($"Category '{name}' does not exist.");
                matchedCategories.Add(category);
            }

            newProduct.Categories = matchedCategories;
            _productRepository.AddProduct(newProduct);

            return _mapper.Map<ProductDTO>(newProduct);
        }


        public bool DeleteProduct(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product != null)
            {
                return _productRepository.DeleteProduct(product);
            }
            return false;
        }

        public ProductDTO GetProductById(int id)
        {
            var product = _productRepository.GetProductById(id);
            return _mapper.Map<ProductDTO>(product);
        }

        public List<ProductDTO> GetProducts()
        {
            var products = _productRepository.GetProducts();
            return _mapper.Map<List<ProductDTO>>(products);
        }

        public ProductDTO UpdateProduct(ProductDTO productDTO)
        {
            var oldProduct = _productRepository.GetProductById(productDTO.Id);
            if (oldProduct == null)
                throw new InvalidOperationException($"Product with ID {productDTO.Id} not found.");

            if (productDTO.Price <= 0)
                throw new InvalidOperationException("Product price must be greater than 0.");

            if (productDTO.Categories == null || !productDTO.Categories.Any())
                throw new InvalidOperationException("At least one category must be provided.");

            var allCategories = _categoryRepository.GetCategories();
            var matchedCategories = new List<Category>();

            foreach (var name in productDTO.Categories)
            {
                var category = allCategories
                    .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (category == null)
                    throw new InvalidOperationException($"Category '{name}' does not exist.");
                matchedCategories.Add(category);
            }

            var newProduct = _mapper.Map<Product>(productDTO);
            newProduct.Categories = matchedCategories;

            _productRepository.UpdateProduct(oldProduct, newProduct);
            return _mapper.Map<ProductDTO>(newProduct);
        }


        public void StockImport(List<StockDTO> stockList)
        {
            foreach (var item in stockList)
            {
                var categoryNames = item.Categories.Select(c => c.Trim().ToLower()).ToList();

                var allCategories = _categoryRepository.GetCategories();
                var matchedCategories = new List<Category>();
                foreach (var name in categoryNames)
                {
                    var category = allCategories.FirstOrDefault(c => c.Name.ToLower() == name);
                    if (category == null)
                    {
                        category = new Category { Name = name };
                        _categoryRepository.AddCategory(category);
                    }
                    matchedCategories.Add(category);
                }
                var existingProduct = _productRepository.GetProducts()
                    .FirstOrDefault(p => p.Name.ToLower() == item.Name.ToLower());
                if (existingProduct != null)
                {
                    existingProduct.Quantity += item.Quantity;
                    foreach (var cat in matchedCategories)
                    {
                        if (!existingProduct.Categories.Any(c => c.Id == cat.Id))
                        {
                            existingProduct.Categories.Add(cat);
                        }
                    }

                    var oldProduct = _productRepository.GetProductById(existingProduct.Id);
                    _productRepository.UpdateProduct(oldProduct, existingProduct);
                }
                else
                {
                    var newProduct = new Product
                    {
                        Name = item.Name,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        Categories = matchedCategories
                    };
                    _productRepository.AddProduct(newProduct);
                }
            }
        }

        public DiscountResultDTO CalculateDiscount(List<CartItemDTO> cart)
        {
            var allProducts = _productRepository.GetProducts();
            decimal total = 0;
            decimal discountTotal = 0;
            var result = new DiscountResultDTO();
            foreach (var item in cart)
            {
                var product = allProducts.FirstOrDefault(p =>
                    p.Name.Equals(item.ProductName, StringComparison.OrdinalIgnoreCase));

                if (product == null)
                    throw new InvalidOperationException($"Product '{item.ProductName}' not found.");

                if (item.Quantity > product.Quantity)
                    throw new InvalidOperationException($"Not enough stock for '{product.Name}'.");

                total += product.Price * item.Quantity;

                if (item.Quantity > 1 && product.Categories.Any())
                {
                    decimal discount = Math.Round(product.Price * 0.05m, 2);
                    discountTotal += discount;

                    result.Discounts.Add(new PerProductDiscountDTO
                    {
                        ProductName = product.Name,
                        DiscountAmount = discount
                    });
                }
            }

            result.TotalBeforeDiscount = Math.Round(total, 2);
            result.TotalDiscount = Math.Round(discountTotal, 2);
            result.FinalPrice = Math.Round(total - discountTotal, 2);
            return result;
        }

    }
}
