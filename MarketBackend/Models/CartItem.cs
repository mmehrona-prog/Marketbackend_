namespace MarketBackend.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        //связка с таблиуей пользователей
        public int UserId { get; set; }
        public User? User { get; set; }
        //связка с таблицей товаров
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}
