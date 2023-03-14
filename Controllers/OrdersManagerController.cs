using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OrdersManager.Models;
using System.Text;

namespace OrdersManager.Controllers
{
    public class OrdersManagerController : Controller
    {
        // Orders overview page
        public async Task<IActionResult> Index()
        {
            
            var dateEndStr = DateTime.Today.ToString("yyyy-MM-dd");
            var dateStart = DateTime.Today.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEndStr;

            var orders = new List<OrderModel>();
            try
            {           
                var uri = @"https://localhost:7063/api/Orders";
                HttpClient client = new HttpClient();
                var responseString = await client.GetStringAsync(uri);
                var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);

                orders = data.Where(e => e.Date > DateTime.Today.AddMonths(-1) && e.Date < DateTime.Today).ToList();

                var pageModel = NewPageModel(orders);
                return View(pageModel);
            }
            catch(Exception ex)
            {
                
            }
            return View();
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
            if(data != null)
            {
                var orders = data.Where(e => e.Date > dateStart && e.Date < dateEnd).ToList();
                result = orders.Where(e => NumberFilter.Contains(e.Number) || ProviderFilter.Contains(e.ProviderId.ToString())).ToList();
            }

            var pageModel = NewPageModel(result);
            
            return View(pageModel);
        }


        public async Task<IActionResult> OrderView(int orderId)
        {
            var uri = @"https://localhost:7063/api/Orders/" + orderId;
            HttpClient client = new HttpClient();
            var responseString = await client.GetStringAsync(uri);
            var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);

            return View(data);
        }       

        [HttpPost]
        public async Task<IActionResult> OrderCreateEdit(OrderModel order)
        {
            var uri = @"https://localhost:7063/api/Orders/EditOrder";
            var serData = JsonConvert.SerializeObject(order);
            HttpClient client = new HttpClient();
            var content = new StringContent(serData, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, content);
            var data = await response.Result.Content.ReadAsStringAsync();

            return View();
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
    }
}
