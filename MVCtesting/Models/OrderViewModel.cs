using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class OrderViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string BrandName { get; set; }
        public int Quantity { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageFileName { get; set; }
    }
}
