namespace MarketBackend.DTOs.Response
{
    public class CartItemViewDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalItemPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

    }
}
