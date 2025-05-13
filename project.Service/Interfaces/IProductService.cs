using System;
using project.Service.DTOs;
namespace project.Service.Interfaces
{

    public interface IProductService
    {
        List<ProductDTO> GetProducts();

        ProductDTO GetProductById(int id);


        ProductDTO AddProduct(ProductDTO productDTO);

        ProductDTO UpdateProduct(ProductDTO productDTO);

        bool DeleteProduct(int id);
        void StockImport(List<StockDTO> stocks);
        DiscountResultDTO CalculateDiscount(List<CartItemDTO> cart);

    }
}
