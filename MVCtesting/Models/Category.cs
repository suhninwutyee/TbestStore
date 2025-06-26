using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCtesting.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }    
        
        [Required]
        public string? Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? ImageUrl { get; set; } 

        public ICollection<Brand>? Brands { get; set; }
        public ICollection<CategoryBrand> CategoryBrands { get; set; } = new List<CategoryBrand>();
        public ICollection<Product> Tproducts { get; set; } = new List<Product>();
    }
}
