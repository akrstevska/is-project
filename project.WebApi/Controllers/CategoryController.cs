using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using project.Data.Interfaces;
using project.Service.DTOs;
using project.Service.Interfaces;
namespace project.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<CategoryDTO> GetCategories()
        {
            var categories = _categoryService.GetCategories();
            return categories;
        }

        [HttpGet]
        [Route("GetCategoryById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryService.GetCategoryById(id);

            if (category == null)
                return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(category);
        }

        [HttpPost]
        [Route("AddCategory")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            try
            {
                var newCategory = _categoryService.AddCategory(category);
                return Created($"Category with id {newCategory.Id} has been created!", newCategory);
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("RemoveCategory/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete([FromRoute] int id)
        {
            CategoryDTO categoryExist = _categoryService.GetCategoryById(id);

            if (categoryExist != null)
            {
                return Ok(new { success = _categoryService.DeleteCategory(id) });
            }
            else
            {
                return BadRequest(new { message = "Category with that id does not exist!" });
            }
        }

        [HttpPut]
        [Route("UpdateCategory/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Put([FromRoute] int id, [FromBody] CategoryDTO category)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var categoryExists = _categoryService.GetCategoryById(id);
            if (categoryExists == null)
                return BadRequest(new { message = "Category with that id does not exist!" });

            category.Id = id;
            var result = _categoryService.UpdateCategory(category);
            return Ok(result);
        }

    }
}
