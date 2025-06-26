
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCtesting.Models
{
    public class ProductDto
    {
        
        public int? product_id { get; set; } 
        [Required, MaxLength(100)]
        public string? Name { get; set; } = "";

        public int BrandId { get; set; } 

        [Required]
        public int? CategoryId { get; set; }

        [Required]
        public decimal? Price { get; set; } 

        [Required]
        public string? Description { get; set; } = "";      
        public IFormFile? ImageFile { get; set; }
        public int Discount { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater")]
        public int Quantity { get; set; }


    }
}
