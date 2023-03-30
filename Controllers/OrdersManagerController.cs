using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OrdersManager.Business_layer;
using OrdersManager.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using static OrdersManager.Routes;

namespace OrdersManager.Controllers
{
    public class OrdersManagerController : Controller
    {
        
        private IOrderManagerService _orderService;
        public OrdersManagerController(IOrderManagerService orderService)
        {
            _orderService = orderService;
        }

        // Orders overview page
        public async Task<IActionResult> Index()
        {

            var dateEndStr = DateTime.Today.ToString("yyyy-MM-dd");
            var dateStart = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEndStr;           

            var orders = new List<OrderModel>();
            var pageModel = NewPageModel(orders);
            try
            {
                // construct API request link
                var uri = Orders.AllOrders;

                GetResponse request = new GetResponse();
                var responseString = await request.Get(uri);

                if(!string.IsNullOrEmpty(responseString))
                {
                    var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
                    orders = data.Where(e => e.Date > DateTime.Today.AddMonths(-1) && e.Date <= DateTime.Now).ToList();

                    pageModel = NewPageModel(orders);
                    return View(pageModel);
                }
                return View();

            }
            catch (Exception ex)
            {
                return View();
            }
            return View(pageModel);
        }

        // Filter orders list
        [HttpPost]
        public async Task<IActionResult> Index(DateTime dateStart, DateTime dateEnd, List<string> NumberFilter, List<string> ProviderFilter)
        {
            dateEnd = dateEnd.AddDays(1);
            ViewBag.dateStart = dateStart.Date.ToString("yyyy-MM-dd");
            ViewBag.dateEnd = dateEnd.Date.ToString("yyyy-MM-dd");                                                              
            var result = new List<OrderModel>();

            // construct API request link
            var uri = Orders.AllOrders;

            GetResponse request = new GetResponse();
            var responseString = await request.Get(uri);

            var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
            if (data != null)
            {
                // Filter by date range
                var orders = data.Where(e => e.Date > dateStart && e.Date <= dateEnd).ToList();
                // Filter by Order fields
                result = orders.Where(e => NumberFilter.Contains(e.Number) || ProviderFilter.Contains(e.ProviderId.ToString())).ToList();
            }

            var pageModel = NewPageModel(result);

            return View(pageModel);
        }

        // Init IndexPageModel
        IndexPageModel NewPageModel(List<OrderModel> orders)
        {

            var a = orders.GroupBy(x => new { x.ProviderId, x.Number }).Select(x => x.First());
            var ordersFilters = _orderService.FilterIndexPage(a);
            List<SelectListItem> numberListSelect = ordersFilters.NumberList.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.ToString(),
                    Value = a.ToString(),
                    Selected = false
                };
            });
            List<SelectListItem> providerListSelect = ordersFilters.ProviderList.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.ToString(),
                    Value = a.ToString(),
                    Selected = false
                };
            });
            return new IndexPageModel(orders, providerListSelect, numberListSelect);
        }


        // OrderCreateEdit init page
        public async Task<IActionResult> OrderCreateEdit(EditCreatePageModel model)
        {
            if (model == null)
                model = new EditCreatePageModel(new OrderModel(), new List<OrderItemModel>());

            // construct API request link
            var uri = Orders.Providers;

            GetResponse request = new GetResponse();
            var responseString = await request.Get(uri);
            if (!string.IsNullOrEmpty(responseString))
            {
                var providers = JsonConvert.DeserializeObject<List<ProviderModel>>(responseString);
                model.Providers = providers;

                return View(model);
            }
            return View("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ProcessFormOrderCreateEdit(EditCreatePageModel model, string? saveOrder, string? addOrderItem, string? deleteItem, int itemId, int Provider)
        {
            model.Order.ProviderId = Provider;
            if (saveOrder != null)
            {
                ModelState.ClearValidationState("NewOrderItem.Name");
                ModelState.ClearValidationState("NewOrderItem.Unit");
                ModelState.MarkFieldValid("NewOrderItem.Unit");
                ModelState.MarkFieldValid("NewOrderItem.Name");
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    // Try save order
                    var saveResult = await _orderService.SaveOrder(model);

                    if (saveResult.IsSuccessful)
                    {
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    if(saveResult.ValidationsErrors.Count > 0)
                    {
                        foreach(var error in saveResult.ValidationsErrors)
                        {
                            error.AddToModelState(this.ModelState);
                        }                       
                    }
                    if(!string.IsNullOrEmpty(saveResult.ErrorCode))
                    {
                        ViewBag.ErrorMsg = saveResult.ErrorMessage;                        
                    }
                    return View("OrderCreateEdit", model);
                }
            }                        
            if (addOrderItem != null)
            {
                return AddOrderItem(model);
            }
            if (deleteItem != null)
            {
                return DeleteOrderItem(model, itemId);
            }
            return View("OrderCreateEdit", model);
        }

        // Remove OrderItem 
        public IActionResult DeleteOrderItem(EditCreatePageModel model, int id)
        {
            model.OrderItems.Remove(model.OrderItems[id]);
            ModelState.Clear();

            return View("OrderCreateEdit", model);
        }

        // Add order item
        private IActionResult AddOrderItem(EditCreatePageModel model)
        {
            var resultValidation = _orderService.ValidateOrderItems(model);            
            if (ModelState.IsValid && resultValidation.IsValid)
            {
                if (model.OrderItems == null)
                    model.OrderItems = new List<OrderItemModel>();
                model.NewOrderItem.Order = model.Order;
                model.OrderItems.Add(model.NewOrderItem);

                ModelState.Clear();
            }
            resultValidation.AddToModelState(this.ModelState);
            return View("OrderCreateEdit", model);
        }

        // View order with OrderItems
        public async Task<IActionResult> ViewOrder(int id)
        {
            // construct API request link
            var uri = Orders.Order;
            
            GetResponse request = new GetResponse();
            var responseString = await request.Get(uri + id);
            if (!string.IsNullOrEmpty(responseString))
            {
                var data = JsonConvert.DeserializeObject<EditCreatePageModel>(responseString);

                return View(data);
            }
            else
            {
                ViewBag.ErrorMsg = "Не удалось загрузить данные заказа";
                return View("ErrorPage");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> ProcessFormOrderView(int id, string? DeleteOrder, string? EditOrder)
        {
            if (ModelState.IsValid)
            {
                if (DeleteOrder != null)
                {
                    GetResponse request = new GetResponse();

                    var uri = Orders.Order;                  

                    var response = await request.Delete(uri, id);
                    var status = response.StatusCode;
                    if (status == HttpStatusCode.OK)
                    {
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.Clear();
                        ViewBag.ErrorMsg = "Неудачная попытка удаления заказа";
                        return View("ErrorPage");
                    }

                }
            }
            if (EditOrder != null)
            {
                var uri = Orders.Order;
                //var uri = @"https://localhost:7063/api/Orders/Order" + id;
                GetResponse request = new GetResponse();
                var responseString = await request.Get(uri+ id);
                if (!string.IsNullOrEmpty(responseString))
                {
                    var model = JsonConvert.DeserializeObject<EditCreatePageModel>(responseString);
                    responseString = null;

                    uri = Orders.Providers;

                    responseString = await request.Get(uri);
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        var providers = JsonConvert.DeserializeObject<List<ProviderModel>>(responseString);
                        model.Providers = providers;
                        ModelState.Clear();
                        return View("OrderCreateEdit", model);
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "Не удалось загрузить данные заказа";
                    return View("ErrorPage");
                }
            }
            return View("Index");
        }
    }
}
