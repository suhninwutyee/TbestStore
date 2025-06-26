using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class CategoryCreateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public List<int> SelectedBrandIds { get; set; } = new List<int>();

        public List<SelectListItem> BrandsSelectList { get; set; } = new List<SelectListItem>();
    }
}
