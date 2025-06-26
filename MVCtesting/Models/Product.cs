using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace MVCtesting.Models
{

   
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public int product_id { get; set; } 

        [MaxLength(100)]
        public string? Name { get; set; } = "";

        [MaxLength(100)]
        //public string? Brand { get; set; } = "";

        //public int Category { get; set; } 
        

        [Precision(16,2)]
        public decimal? Price { get; set; } 
        public string? Description { get; set; } = "";

        [MaxLength(100)]
        public string? ImageFileName { get; set; } = "";
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("BrandId")]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public int Discount { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or more")]
        public int Quantity { get; set; }

    }
}
