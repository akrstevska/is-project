using System;
using System.ComponentModel.DataAnnotations;

namespace project.Service.DTOs
{

    public class CartItemDTO
    {
        public string ProductName { get; set; }
        
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

    }
}
