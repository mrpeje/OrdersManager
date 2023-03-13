using OrdersManager.Models;

namespace OrdersManager.Layers.Validator
{
    public interface IValidator
    {
        public bool ValidateOrder(OrderModel order);

    }
}
