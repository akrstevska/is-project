using AutoMapper;
using Microsoft.EntityFrameworkCore;
using project.Data;
using project.Data.Entities;
using project.Data.Repositories;
using project.Service.DTOs;
using project.Service.Services;
using project.Service.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace project.Tests.Integration
{
    public class CategoryServiceIntegrationTests
    {
        private DbContextOptions<ProjectContext> GetInMemoryDbOptions()
        {
            return new DbContextOptionsBuilder<ProjectContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
            });
            return config.CreateMapper();
        }

        [Fact]
        public void AddCategory_ValidInput_PersistsToDatabase()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            var repo = new CategoryRepository(context);
            var mapper = GetMapper();
            var service = new CategoryService(repo, mapper);

            var dto = new CategoryDTO { Name = "Monitors", Description = "Computer monitors description" };
            var result = service.AddCategory(dto);

            var saved = context.Categories.FirstOrDefault();
            Assert.NotNull(saved);
            Assert.Equal("Monitors", saved.Name);
        }

        [Fact]
        public void AddCategory_DuplicateName_ThrowsException()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            context.Categories.Add(new Category { Name = "Storage" });
            context.SaveChanges();

            var repo = new CategoryRepository(context);
            var mapper = GetMapper();
            var service = new CategoryService(repo, mapper);

            var dto = new CategoryDTO { Name = "Storage" };
            var ex = Assert.Throws<InvalidOperationException>(() => service.AddCategory(dto));
            Assert.Equal("Category with the same name exists.", ex.Message);
        }

        [Fact]
        public void DeleteCategory_ExistingId_RemovesFromDatabase()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            var category = new Category { Name = "Keyboards" };
            context.Categories.Add(category);
            context.SaveChanges();

            var repo = new CategoryRepository(context);
            var mapper = GetMapper();
            var service = new CategoryService(repo, mapper);

            var result = service.DeleteCategory(category.Id);
            Assert.True(result);
            Assert.Empty(context.Categories);
        }

        [Fact]
        public void UpdateCategory_ValidInput_UpdatesSuccessfully()
        {
            var options = GetInMemoryDbOptions();
            using var context = new ProjectContext(options);
            var category = new Category { Name = "Printers", Description = "Old description" };
            context.Categories.Add(category);
            context.SaveChanges();

            var repo = new CategoryRepository(context);
            var mapper = GetMapper();
            var service = new CategoryService(repo, mapper);

            var updateDto = new CategoryDTO
            {
                Id = category.Id,
                Name = "Printers",
                Description = "New description"
            };

            var updated = service.UpdateCategory(updateDto);

            Assert.Equal("New description", updated.Description);
        }
    }
}
