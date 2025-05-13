using System;

namespace project.Service.DTOs
{

    public class StockDTO
    {
        public string Name { get; set; }
        public List<string> Categories { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
