namespace MVCtesting.Models
{
    public class ProductListViewModel
    {
        public List<Product> Products { get; set; }
        public int TotalCount { get; set; }
        public string SearchString { get; set; }
        public DateTime? SearchDate { get; set; }
        public int? CategoryId { get; set; }
    }
}
