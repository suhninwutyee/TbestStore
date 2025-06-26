using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to Order
        [Required]
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

        // Foreign key to Product
        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        // Price of the product at time of order
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Quantity ordered
        [Required]
        public int Quantity { get; set; }
    }
}
