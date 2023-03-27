using OrdersManager.Models;

namespace OrdersManager.Business_layer
{
    public interface IOrderManagerService
    {
        public FilterModel FilterIndexPage(IEnumerable<OrderModel> orders);
    }
}
