using FluentValidation.Results;
using OrdersManager.Models;

namespace OrdersManager.Business_layer
{
    public interface IOrderManagerService
    {
        public FilterModel FilterIndexPage(IEnumerable<OrderModel> orders);
        public Task<SaveResult> SaveOrder(EditCreatePageModel model);
        public ValidationResult ValidateOrderItems(EditCreatePageModel model);
        public Task<ValidationResult> ValidateOrder(EditCreatePageModel model);
    }
}
