using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCtesting.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public ApplicationUser Customer { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string? PaymentMethod { get; set; } // Add this if not already there
        public ICollection<OrderItem> OrderItems { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }  // ✅ Add this line

        public decimal TotalAmount { get; set; }
    }
}

