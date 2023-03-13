namespace OrdersManager.Models
{
    public class OrderItemModel
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }

        public OrderModel Order { get; set; }
    }
}
