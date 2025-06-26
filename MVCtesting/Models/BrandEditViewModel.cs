using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class BrandEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Brand Name")]
        public string Name { get; set; }

        [Display(Name = "Select Categories")]
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<SelectListItem> CategoriesSelectList { get; set; } = new List<SelectListItem>();
    }
}
