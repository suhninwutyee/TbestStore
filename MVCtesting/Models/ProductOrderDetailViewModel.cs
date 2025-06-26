namespace MVCtesting.Models
{
    public class ProductOrderDetailViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount => (Product.Price ?? 0) * Quantity;
    }
}
