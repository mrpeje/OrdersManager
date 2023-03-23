using FluentValidation;
using Newtonsoft.Json;
using OrdersManager.Models;
using static OrdersManager.Routes;

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

                RuleFor(m => m.OrderItems).Must((model, item) => Check(model, item))
                .WithMessage("Имя элемента заказа не может совпадать с номером заказа");


            });
            RuleSet("Order", () =>
            {
                RuleFor(m => m.Order).MustAsync((order, cancellation) => ValidateOrderNumberAsync(order))
                .WithMessage(e=>$"Заказ с номером:{e.Order.Number} уже существует для провайдера {e.Providers.FirstOrDefault(x=>x.Id == e.Order.ProviderId).Name}");
            });
        }

        private bool Check(EditCreatePageModel model, List<OrderItemModel> list)
        {
            if(model.OrderItems != null)
                foreach(var item in model.OrderItems)
                {
                    if(item.Name == model.Order.Number)
                    {
                        return false;
                    }
                }
            return true;
        }


        private async Task<bool> ValidateOrderNumberAsync(OrderModel order)
        {
            GetResponse request = new GetResponse();
            var uri = Orders.Providers;

            var responseString = await request.Get(uri);
            if (!string.IsNullOrEmpty(responseString))
            {
                var ordersByProvider = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
                bool result = !ordersByProvider.Any(e => e.Number == order.Number);

                foreach (var item in ordersByProvider)
                {
                    if (item.Number == order.Number && item.Id != order.Id)
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
