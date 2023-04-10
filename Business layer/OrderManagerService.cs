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

            SaveResult saveResult;

            if (resultValidationOrder.IsValid && resultValidationOrderItems.IsValid)
            {
                saveResult = await UpdateOrderItems(model);
                
                if (saveResult.IsSuccessful)
                {

                }
                else
                {

                }
            }
            else
            {
                saveResult = new SaveResult();
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
        async Task<SaveResult> UpdateOrderItems(EditCreatePageModel dataModel)
        {
            var result = new SaveResult();
            try
            {
                GetResponse request = new GetResponse();
                // construct API request link
                var uri = OrderManager.Items;
                var responseString = await request.Get(uri + dataModel.Order.Id);
                var orderItems = JsonConvert.DeserializeObject<List<OrderItemModel>>(responseString);

                if (dataModel.OrderItems != null)
                {
                    // construct API request link
                    var itemUri = OrderManager.Item;
                    // Update or Add OrderItems
                    foreach (var item in dataModel.OrderItems)
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
                            var response = await requestitem.Post(itemUri + dataModel.Order.Id, JsonItem);
                            var status = response.StatusCode;
                            if (status != HttpStatusCode.OK)
                            {

                            }
                        }
                    }
                    foreach (var item in dataModel.OrderItems)
                    {
                        item.Order = dataModel.Order;
                    }
                    // Delete OrderItems                  
                     
                    var deletedItems = orderItems.Where(userItem => dataModel.OrderItems.All(dbItem => userItem.id != dbItem.id)).ToList();
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
                else
                {
                    foreach (var item in orderItems)
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

                result.IsSuccessful = false;
                result.ErrorMessage = "Ошибка при сохранении заказа";
                return result;
            }
            result.IsSuccessful = true;
            return result;
        }
    }
}
