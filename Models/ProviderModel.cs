namespace OrdersManager.Models
{
    public class ProviderModel
    {
        public string Name { get; set; }

        public ICollection<OrderModel> Order { get; set; }
    }
}
