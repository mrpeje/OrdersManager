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
            var resultValidationOrder = await ValidateOrder(model);
            var resultValidationOrderItems =  ValidateOrderItems(model);
            var validationErrors = new List<ValidationResult>();

            SaveResult saveResult = new SaveResult();

            if (resultValidationOrder.IsValid && resultValidationOrderItems.IsValid)
            {
                saveResult.IsSuccessful = await UpdateOrder(model.Order);
                if (!saveResult.IsSuccessful)
                    saveResult.IsSuccessful = await CreateOrder(model.Order);
                if (saveResult.IsSuccessful)
                    saveResult.IsSuccessful = await UpdateOrderItems(model.Order, model.OrderItems);
                
                if (!saveResult.IsSuccessful)
                {
                    saveResult.ErrorMessage = "Ошибка при сохранении заказа в БД";
                }
            }
            else
            {
                saveResult.IsSuccessful = false;
                validationErrors.Add(resultValidationOrder);
                validationErrors.Add(resultValidationOrderItems);
                saveResult.ValidationsErrors = validationErrors;
            }
           
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
        async Task<bool> CreateOrder(OrderModel order)
        {
            GetResponse request = new GetResponse();
            // construct API request link
            var uri = OrderManager.Order;
            var jsonOrder = JsonConvert.SerializeObject(order);
            var response = await request.Post(uri, jsonOrder);
            var status = response.StatusCode;
            if (status != HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }
        async Task<bool> UpdateOrder(OrderModel order)
        {
            GetResponse request = new GetResponse();
            // construct API request link
            var uri = OrderManager.Order;
            var jsonOrder = JsonConvert.SerializeObject(order);
            var response = await request.Put(uri, jsonOrder);
            var status = response.StatusCode;
            if (status != HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }
        async Task<bool> UpdateOrderItems(OrderModel order, List<OrderItemModel> userOrderItems)
        {
            try
            {
                GetResponse request = new GetResponse();
                // construct API request link
                var uri = OrderManager.Items;
                // Get all items for this order
                var responseString = await request.Get(uri + order.Id);
                var dbOrderItems = JsonConvert.DeserializeObject<List<OrderItemModel>>(responseString);

                if (userOrderItems != null)
                {
                    // construct API request link
                    var itemUri = OrderManager.Item;
                    // Update or Add OrderItems
                    foreach (var item in userOrderItems)
                    {
                        GetResponse requestitem = new GetResponse();
                        var JsonItem = JsonConvert.SerializeObject(item);
                        if (item.id != 0)
                        { 
                            var response = await requestitem.Put(itemUri, JsonItem);
                            var status = response.StatusCode;
                            if (status != HttpStatusCode.OK)
                            {

                            }
                        }
                        else
                        {
                            var response = await requestitem.Post(itemUri + order.Id, JsonItem);
                            var status = response.StatusCode;
                            if (status != HttpStatusCode.OK)
                            {

                            }
                        }
                    }
                    // Delete OrderItems                  
                     
                    var deletedItems = dbOrderItems.Where(dbItem => userOrderItems.All(userItem => dbItem.id != userItem.id)).ToList();
                    foreach (var item in deletedItems)
                    {
                        GetResponse requestDelete = new GetResponse();
                        // construct API request link
                        var deleteItemUri = OrderManager.Item;

                        var response = await requestDelete.Delete(deleteItemUri, item.id);
                        var status = response.StatusCode;
                        if (status != HttpStatusCode.OK)
                        {

                        }
                    }
                }
                // If userOrderItems empty - delete all OrderItems from DB 
                else
                {
                    foreach (var item in dbOrderItems)
                    {
                        GetResponse requestDelete = new GetResponse();
                        // construct API request link
                        var deleteItemUri = OrderManager.Item;

                        var response = await requestDelete.Delete(deleteItemUri, item.id);
                        var status = response.StatusCode;
                        if (status != HttpStatusCode.OK)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }

            return true;
        }
    }
}
