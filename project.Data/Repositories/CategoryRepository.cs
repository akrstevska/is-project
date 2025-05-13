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
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProjectContext _projectContext;
        public CategoryRepository(ProjectContext projectContext)
        {
            _projectContext = projectContext;
        }

        public void AddCategory(Category category)
        {
            _projectContext.Categories.Add(category);
            _projectContext.SaveChanges();
        }

        public bool DeleteCategory(Category category)
        {
            try
            {
                _projectContext.Categories.Remove(category);
                _projectContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ICollection<Category> GetCategories()
        {
            return _projectContext.Categories
                .Include(c => c.Products) 
                .ToList();
        }

        public Category GetCategoryById(int id)
        {
            return _projectContext.Categories
                .Include(c => c.Products) 
                .FirstOrDefault(c => c.Id == id);
        }

        public void UpdateCategory(Category oldCategory, Category newCategory)
        {
            newCategory.Id = oldCategory.Id;

            _projectContext.Entry(oldCategory).State = EntityState.Detached;
            _projectContext.Categories.Attach(newCategory);
            _projectContext.Entry(newCategory).State = EntityState.Modified;

            _projectContext.SaveChanges();
        }
    }
}


