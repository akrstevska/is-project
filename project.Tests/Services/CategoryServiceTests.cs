using AutoMapper;
using Moq;
using project.Data.Entities;
using project.Data.Interfaces;
using project.Service.DTOs;
using project.Service.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace project.Tests.Services
{
    public class CategoryServiceTests
    {
        ICategoryRepository categoryRepo;
        IMapper mapper;

        Mock<ICategoryRepository> categoryRepositoryMock = new();
        Mock<IMapper> mapperMock = new();

        Category category;
        CategoryDTO categoryDTO;
        List<Category> categoryList = new();
        List<CategoryDTO> categoryDTOList = new();

        private Category GetCategory()
        {
            return new Category
            {
                Id = 1,
                Name = "Keyboards",
                Description = "Keyboard products"
            };
        }

        private CategoryDTO GetCategoryDTO()
        {
            return new CategoryDTO
            {
                Id = 1,
                Name = "Keyboards",
                Description = "Keyboard products"
            };
        }

        private List<Category> GetCategories()
        {
            return new List<Category>
            {
                new Category { Id = 1, Name = "Keyboards", Description = "Keyboard products" },
                new Category { Id = 2, Name = "Mice", Description = "Mouse products" }
            };
        }

        private List<CategoryDTO> GetCategoryDTOs()
        {
            return new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Keyboards", Description = "Keyboard products" },
                new CategoryDTO { Id = 2, Name = "Mice", Description = "Mouse products" }
            };
        }

        private void SetupMocks()
        {
            categoryRepo = categoryRepositoryMock.Object;
            mapper = mapperMock.Object;
        }

        private void SetupCategoryDTOListMock()
        {
            categoryList = GetCategories();
            categoryDTOList = GetCategoryDTOs();

            mapperMock.Setup(m => m.Map<List<CategoryDTO>>(categoryList)).Returns(categoryDTOList);
        }

        private void SetupCategoryDTOMock()
        {
            category = GetCategory();
            categoryDTO = GetCategoryDTO();

            mapperMock.Setup(m => m.Map<CategoryDTO>(category)).Returns(categoryDTO);
        }

        [Fact]
        public void AddCategory_WithExistingName_ThrowsInvalidOperationException()
        {
            // Arrange
            SetupMocks();
            categoryDTO = new CategoryDTO { Name = "Keyboards", Description = "New desc" };
            category = new Category { Name = "Keyboards", Description = "Existing" };

            var existingCategories = new List<Category> { category };

            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(existingCategories);
            mapperMock.Setup(m => m.Map<Category>(categoryDTO)).Returns(category);

            var service = new CategoryService(categoryRepo, mapper);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddCategory(categoryDTO));
            Assert.Equal("Category with the same name exists.", ex.Message);
        }

        [Fact]
        public void AddCategory_ValidCategory_ReturnsAddedCategory()
        {
            // Arrange
            SetupMocks();
            categoryDTO = new CategoryDTO { Name = "Speakers", Description = "Audio" };
            category = new Category { Name = "Speakers", Description = "Audio" };

            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(new List<Category>());
            mapperMock.Setup(m => m.Map<Category>(categoryDTO)).Returns(category);
            mapperMock.Setup(m => m.Map<CategoryDTO>(category)).Returns(categoryDTO);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.AddCategory(categoryDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Speakers", result.Name);
            categoryRepositoryMock.Verify(r => r.AddCategory(category), Times.Once);
        }

        [Fact]
        public void GetCategories_ReturnsMappedList()
        {
            // Arrange
            SetupMocks();
            SetupCategoryDTOListMock();
            categoryRepositoryMock.Setup(r => r.GetCategories()).Returns(categoryList);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.GetCategories();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetCategoryById_ValidId_ReturnsCategoryDTO()
        {
            // Arrange
            SetupMocks();
            SetupCategoryDTOMock();

            categoryRepositoryMock.Setup(r => r.GetCategoryById(1)).Returns(category);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.GetCategoryById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Keyboards", result.Name);
        }

        [Fact]
        public void DeleteCategory_ExistingId_ReturnsTrue()
        {
            // Arrange
            SetupMocks();
            category = GetCategory();

            categoryRepositoryMock.Setup(r => r.GetCategoryById(1)).Returns(category);
            categoryRepositoryMock.Setup(r => r.DeleteCategory(category)).Returns(true);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.DeleteCategory(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UpdateCategory_ValidUpdate_ReturnsUpdatedDTO()
        {
            // Arrange
            SetupMocks();
            category = GetCategory();
            categoryDTO = GetCategoryDTO();
            categoryDTO.Description = "Updated Description";

            categoryRepositoryMock.Setup(r => r.GetCategoryById(categoryDTO.Id)).Returns(category);
            categoryRepositoryMock.Setup(r => r.UpdateCategory(category, category));
            mapperMock.Setup(m => m.Map<CategoryDTO>(category)).Returns(categoryDTO);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.UpdateCategory(categoryDTO);

            // Assert
            Assert.Equal("Updated Description", result.Description);
            categoryRepositoryMock.Verify(r => r.UpdateCategory(category, category), Times.Once);
        }

        [Fact]
        public void UpdateCategory_WhenCategoryDoesNotExist_ThrowsException()
        {
            // Arrange
            SetupMocks();
            categoryDTO = new CategoryDTO { Id = 999, Name = "NonExistent", Description = "..." };

            categoryRepositoryMock.Setup(r => r.GetCategoryById(categoryDTO.Id)).Returns((Category)null);

            var service = new CategoryService(categoryRepo, mapper);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateCategory(categoryDTO));
            Assert.Equal("Category with ID 999 not found.", ex.Message);

            categoryRepositoryMock.Verify(r => r.UpdateCategory(It.IsAny<Category>(), It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public void DeleteCategory_CategoryNotFound_ReturnsFalse()
        {
            // Arrange
            SetupMocks();
            categoryRepositoryMock.Setup(r => r.GetCategoryById(99)).Returns((Category)null);

            var service = new CategoryService(categoryRepo, mapper);

            // Act
            var result = service.DeleteCategory(99);

            // Assert
            Assert.False(result);
            categoryRepositoryMock.Verify(r => r.DeleteCategory(It.IsAny<Category>()), Times.Never);
        }


    }
}
