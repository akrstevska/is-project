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

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public CategoryDTO AddCategory(CategoryDTO categoryDTO)
        {
            var newCategory = _mapper.Map<Category>(categoryDTO);
            var existing = _categoryRepository.GetCategories()
                .FirstOrDefault(c => c.Name.ToLower() == newCategory.Name.ToLower());

            if (existing != null)
            {
                throw new InvalidOperationException("Category with the same name exists.");
            }
            _categoryRepository.AddCategory(newCategory);

            return _mapper.Map<CategoryDTO>(newCategory);
        }

        public bool DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category != null)
            {
                return _categoryRepository.DeleteCategory(category);
            }

            return false;
        }

        public List<CategoryDTO> GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            return _mapper.Map<List<CategoryDTO>>(categories);

        }

        public CategoryDTO GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public CategoryDTO UpdateCategory(CategoryDTO categoryDTO)
        {
            var oldCategory = _categoryRepository.GetCategoryById(categoryDTO.Id);

            if (oldCategory == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryDTO.Id} not found.");
            }

            oldCategory.Name = categoryDTO.Name;
            oldCategory.Description = categoryDTO.Description;
            _categoryRepository.UpdateCategory(oldCategory, oldCategory);

            var updatedCategory = _categoryRepository.GetCategoryById(categoryDTO.Id);
            return _mapper.Map<CategoryDTO>(updatedCategory);
        }
    }
}
