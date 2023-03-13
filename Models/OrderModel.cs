namespace OrdersManager.Models
{
    public class OrderModel
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public int ProviderId { get; set; }

        public ProviderModel Provider { get; set; }
    }
}
