using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class CategoryBrand
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }
}
