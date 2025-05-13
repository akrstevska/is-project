using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Data.Interfaces;
using project.Service.DTOs;
using project.Service.Interfaces;

namespace project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("AddProduct")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            try
            {
                var newProduct = _productService.AddProduct(product);
                return Created($"Product with id {newProduct.Id} has been created!", newProduct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<ProductDTO> GetProducts()
        {
            var products = _productService.GetProducts();
            return products;
        }

        [HttpGet]
        [Route("GetProductById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetProductById(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });
            return Ok(product);
        }

        [HttpDelete("RemoveProduct/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete([FromRoute] int id)
        {
            ProductDTO productExist = _productService.GetProductById(id);
            if (productExist != null)
            {
                return Ok(new { success = _productService.DeleteProduct(id) });
            }
            else
            {
                return BadRequest(new { message = "Product with that id does not exist!" });
            }
        }


        [HttpPut]
        [Route("UpdateProduct/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Put([FromRoute] int id, [FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var productExist = _productService.GetProductById(id);
            if (productExist == null)
                return BadRequest(new { message = "Product with that id does not exist!" });
            product.Id = id;
            try
            {
                var result = _productService.UpdateProduct(product);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("ImportStock")]
        public IActionResult StockImport([FromBody] List<StockDTO> stocks)
        {
            try
            {
                _productService.StockImport(stocks);
                return Ok(new { message = "Stock imported successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CalculateDiscount")]
        public IActionResult CalculateDiscount([FromBody] List<CartItemDTO> cart)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            try
            {
                var result = _productService.CalculateDiscount(cart);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
