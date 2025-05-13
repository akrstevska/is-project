using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.Data.Entities;
using project.Data.Interfaces;

namespace project.Data.Repositories
{

    public class ProductRepository : IProductRepository
    {
        private readonly ProjectContext _projectContext;
        public ProductRepository(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public void AddProduct(Product product)
        {
            _projectContext.Products.Add(product);
            _projectContext.SaveChanges();
        }

        public bool DeleteProduct(Product product)
        {
            try
            {
                _projectContext.Products.Remove(product);
                _projectContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ICollection<Product> GetProducts()
        {
            return _projectContext.Products
                .Include(c => c.Categories)
                .ToList();
        }

        public Product GetProductById(int id)
        {
            return _projectContext.Products
                .Include(c => c.Categories)
                .FirstOrDefault(c => c.Id == id);
        }

        public void UpdateProduct(Product oldProduct, Product newProduct)
        {
            newProduct.Id = oldProduct.Id;

            _projectContext.Entry(oldProduct).State = EntityState.Detached;
            _projectContext.Products.Attach(newProduct);
            _projectContext.Entry(newProduct).State = EntityState.Modified;

            _projectContext.SaveChanges();
        }
    }
}
