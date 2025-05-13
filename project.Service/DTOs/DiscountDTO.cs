using System;

namespace project.Service.DTOs
{

    public class DiscountResultDTO
    {
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal FinalPrice { get; set; }
        public List<PerProductDiscountDTO> Discounts { get; set; } = new();
    }
    public class PerProductDiscountDTO
    {
        public string ProductName { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
