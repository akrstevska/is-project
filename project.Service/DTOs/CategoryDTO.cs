using System;
using project.Data.Entities;

namespace project.Service.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; } 

        public string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<ProductDTO> Products { get; set; } = new List<ProductDTO>();
    }
}


