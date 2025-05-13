using System;
using project.Data.Entities;

namespace project.Data.Interfaces
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategoryById(int id);

        void AddCategory(Category category);

        void UpdateCategory(Category oldCategory, Category newCategory);

        bool DeleteCategory(Category category);

    }
}
