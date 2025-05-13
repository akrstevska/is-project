using System;
using project.Data.Entities;

namespace project.Data.Interfaces
{

    public interface IProductRepository
    {
        ICollection<Product> GetProducts();
        Product GetProductById(int id);

        void AddProduct(Product product);

        void UpdateProduct(Product oldProduct, Product newProduct);

        bool DeleteProduct(Product product);
    }
}
