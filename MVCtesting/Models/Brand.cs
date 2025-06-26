using System.ComponentModel.DataAnnotations.Schema;

namespace MVCtesting.Models
{
    public class Brand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<CategoryBrand> CategoryBrands { get; set; } = new List<CategoryBrand>();
    }
}
