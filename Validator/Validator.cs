using FluentValidation;
using Newtonsoft.Json;
using OrdersManager.Models;

namespace OrdersManager.Validator
{

    public class OrderValidator : AbstractValidator<EditCreatePageModel>
    {
        public OrderValidator()
        {
            RuleSet("OrderItems", () =>
            {
                RuleFor(m => m.NewOrderItem).Must((model, item) => model.Order.Number != item.Name)
                .WithMessage("Имя элемента заказа не может совпадать с номером заказа");
            });
            RuleSet("Order", () =>
            {
                RuleFor(m => m.Order).MustAsync((order, cancellation) => ValidateOrderNumberAsync(order)).WithMessage("Заказ с таким номером уже существует для указанного провайдера");
            });
        }
        private async Task<bool> ValidateOrderNumberAsync(OrderModel order)
        {
            GetResponse request = new GetResponse();
            var uri = @"https://localhost:7063/api/Orders/provider/" + order.ProviderId.ToString();
            var responseString = await request.Get(uri);

            var ordersByProvider = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
            bool result = !ordersByProvider.Any(e => e.Number == order.Number);

            foreach (var item in ordersByProvider)
            {
                if (item.Number == order.Number && item.Id != order.Id)
                    return false;
            }
            return true;
        }
    }
}
