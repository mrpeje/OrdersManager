using OrdersManager.Models;

namespace OrdersManager.Layers.Validator
{
    public class Validator : IValidator
    {
        public bool ValidateOrder(OrderModel order)
        {
            return true;
        }
    }
}
