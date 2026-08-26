namespace MarketBackend.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        //связь с главным заказом
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        //связ с товаром
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        
    }
}
