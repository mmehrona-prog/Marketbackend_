namespace MarketBackend.Models
{
    public class Order
    {
        public int Id { get; set; }
        //связка с покупателем
        public int UserId { get; set; }
        public User? User { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        //связка, у одного заказа есть списко позиций
        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
