namespace MVCtesting.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } // or CustomerId
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
