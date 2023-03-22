using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OrdersManager.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace OrdersManager.Controllers
{
    public class OrdersManagerController : Controller
    {

        private IValidator<EditCreatePageModel> _validator;
        public OrdersManagerController(IValidator<EditCreatePageModel> validator)
        {
            _validator = validator;
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
                GetResponse request = new GetResponse();
                var uri = @"https://localhost:7063/api/Orders";
                var responseString = await request.Get(uri);

                var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);

                orders = data.Where(e => e.Date > DateTime.Today.AddMonths(-1) && e.Date <= DateTime.Today).ToList();

                pageModel = NewPageModel(orders);
                return View(pageModel);
            }
            catch (Exception ex)
            {

            }
            return View(pageModel);
        }

        // Filter orders list
        [HttpPost]
        public async Task<IActionResult> Index(DateTime dateStart, DateTime dateEnd, List<string> NumberFilter, List<string> ProviderFilter)
        {
            ViewBag.dateStart = dateStart.Date.ToString("yyyy-MM-dd");
            ViewBag.dateEnd = dateEnd.Date.ToString("yyyy-MM-dd");
            var result = new List<OrderModel>();
            var uri = @"https://localhost:7063/api/Orders";
            HttpClient client = new HttpClient();

            var responseString = await client.GetStringAsync(uri);
            var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
            if (data != null)
            {
                // Filter by date range
                var orders = data.Where(e => e.Date > dateStart && e.Date < dateEnd).ToList();
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
            var providerList = new List<int>();
            var numberList = new List<string>();
            foreach (var b in a)
            {
                providerList.Add(b.ProviderId);
                numberList.Add(b.Number);
            }
            List<SelectListItem> numberListSelect = numberList.ConvertAll(a =>
            {
                return new SelectListItem()
                {
                    Text = a.ToString(),
                    Value = a.ToString(),
                    Selected = false
                };
            });
            List<SelectListItem> providerListSelect = providerList.ConvertAll(a =>
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
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ProcessFormOrderCreateEdit(EditCreatePageModel model, string? saveOrder, string? addOrderItem)
        {
            if (saveOrder != null)
            {
                
                return await OrderSave(model);
            }                        
            if (addOrderItem != null)
            {
                return AddOrderItem(model, model.NewOrderItem);
            }           
            return View("OrderCreateEdit", model);
        }

        // Remove OrderItem 
        [HttpPost]
        public IActionResult DeleteOrderItem(EditCreatePageModel model, int id)
        {
            if (ModelState.IsValid)
            {
                model.OrderItems.Remove(model.OrderItems[id]);
                ModelState.Clear();
            }
            return View("OrderCreateEdit", model);
        }

        // Save order
        private async Task<IActionResult> OrderSave(EditCreatePageModel model)
        {
            ModelState.ClearValidationState("NewOrderItem.Name");
            ModelState.ClearValidationState("NewOrderItem.Unit");
            ModelState.MarkFieldValid("NewOrderItem.Unit");
            ModelState.MarkFieldValid("NewOrderItem.Name");
            var resultValidation = await _validator.ValidateAsync(model, options => options.IncludeRuleSets("Order"));
            resultValidation.AddToModelState(this.ModelState);


            if (ModelState.IsValid)
            {
                GetResponse request = new GetResponse();
                var uri = @"https://localhost:7063/api/Orders/CreateEditOrder";
                var serializedData = JsonConvert.SerializeObject(model);

                var response = await request.Post(uri, serializedData);
                var status = response.StatusCode;
                if (status == HttpStatusCode.OK)
                {
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
            }
            return View("OrderCreateEdit", model);

        }

        // Add order item
        private IActionResult AddOrderItem(EditCreatePageModel model, OrderItemModel newItem)
        {
            var resultValidation = _validator.Validate(model, options => options.IncludeRuleSets("OrderItems"));
            
            //ModelState.ClearValidationState("Order");
            if (ModelState.IsValid && resultValidation.IsValid)
            {
                if (model.OrderItems == null)
                    model.OrderItems = new List<OrderItemModel>();
                newItem.Order = model.Order;
                model.OrderItems.Add(newItem);

                ModelState.Clear();
            }
            resultValidation.AddToModelState(this.ModelState);
            return View("OrderCreateEdit", model);
        }

        // View order with OrderItems
        public async Task<IActionResult> ViewOrder(int id)
        {
            var uri = @"https://localhost:7063/api/Orders/" + id;
            GetResponse request = new GetResponse();
            var responseString = await request.Get(uri);
            
            var data = JsonConvert.DeserializeObject<EditCreatePageModel>(responseString);

            return View(data);
        }
        
        [HttpPost]
        public async Task<IActionResult> ProcessFormOrderView(int id, string? DeleteOrder, string? EditOrder)
        {
            if(ModelState.IsValid)
            if (DeleteOrder != null)
            {
                GetResponse request = new GetResponse();
                var uri = @"https://localhost:7063/api/Orders/Order/";

                var response = await request.Delete(uri, id);
                var status = response.StatusCode;
                if (status == HttpStatusCode.OK)
                {
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }               
            }
            if (EditOrder != null)
            {
                var uri = @"https://localhost:7063/api/Orders/" + id;
                GetResponse request = new GetResponse();
                var responseString = await request.Get(uri);

                var model = JsonConvert.DeserializeObject<EditCreatePageModel>(responseString);
                return View("OrderCreateEdit", model);
            }
            return View("Index");
        }
    }
}
