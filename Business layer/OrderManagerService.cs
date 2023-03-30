using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using OrdersManager.Models;
using System.Net;
using static OrdersManager.Routes;

namespace OrdersManager.Business_layer
{
    public class OrderManagerService : IOrderManagerService
    {
        private IValidator<EditCreatePageModel> _validator;

        public OrderManagerService(IValidator<EditCreatePageModel> validator)
        {
            _validator = validator;
        }

        // return filters for indexPage
        public FilterModel FilterIndexPage(IEnumerable<OrderModel> orders)
        {
            var newIndexFilter = new FilterModel
            {
                NumberList = new List<string>(),
                ProviderList = new List<int>()
            };

            foreach (var item in orders)
            {
                newIndexFilter.ProviderList.Add(item.ProviderId);
                newIndexFilter.NumberList.Add(item.Number);
            }

            newIndexFilter.ProviderList = newIndexFilter.ProviderList.Distinct().ToList();
            newIndexFilter.NumberList = newIndexFilter.NumberList.Distinct().ToList();

            return newIndexFilter;
        }
        public async Task<SaveResult> SaveOrder(EditCreatePageModel model)
        {
            var saveResult = new SaveResult();
            var validationErrors = new List<ValidationResult>();

            var resultValidationOrder = await ValidateOrder(model);

            var resultValidationOrderItems =  ValidateOrderItems(model);


            if (resultValidationOrder.IsValid && resultValidationOrderItems.IsValid)
            {
                // construct API request link
                var uri = Orders.CreateEditOrder;

                GetResponse request = new GetResponse();
                var serializedData = JsonConvert.SerializeObject(model);

                var response = await request.Post(uri, serializedData);
                var statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    saveResult.IsSuccessful = true;

                }
                else
                {
                    saveResult.IsSuccessful = false;
                    saveResult.ErrorMessage = "Ошибка при сохранении заказа";
                    saveResult.ErrorCode = statusCode.ToString();
                }
            }
            else
            {
                validationErrors.Add(resultValidationOrder);
                validationErrors.Add(resultValidationOrderItems);
            }
            saveResult.ValidationsErrors = validationErrors;
            return saveResult;
        }
        public ValidationResult ValidateOrderItems(EditCreatePageModel model)
        {
            return _validator.Validate(model, options => options.IncludeRuleSets("OrderItems"));
        }
        public async Task<ValidationResult> ValidateOrder(EditCreatePageModel model)
        {
            return await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Order"));
        }
    }
}
