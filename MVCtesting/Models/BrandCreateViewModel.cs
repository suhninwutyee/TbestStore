using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCtesting.Models
{
    public class BrandCreateViewModel
    {
        public int?Id { get; set; }

        public string Name { get; set; }
        public List<int> SelectedCategoryIds { get; set; } = new List<int>();
        public List<SelectListItem> CategoriesSelectList { get; set; } = new List<SelectListItem>();



    }
}
