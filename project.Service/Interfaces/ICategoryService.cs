using System;
using project.Service.DTOs;
namespace project.Service.Interfaces
{

    public interface ICategoryService
    {
        List<CategoryDTO> GetCategories();

        CategoryDTO GetCategoryById(int id);


        CategoryDTO AddCategory(CategoryDTO categoryDTO);

        CategoryDTO UpdateCategory(CategoryDTO categoryDTO);

        bool DeleteCategory(int id);
    }
}
