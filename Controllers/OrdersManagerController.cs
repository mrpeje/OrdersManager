using Microsoft.AspNetCore.Mvc;
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
            var dateStart = new DateTime(2023, 1, 1).ToString("yyyy-MM-dd");
            var dateEndStr = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.dateStart = dateStart;
            ViewBag.dateEnd = dateEndStr;

            var orders = new List<OrderModel>();
            try
            {           
                var uri = @"https://localhost:7063/api/Orders";
                HttpClient client = new HttpClient();
                var responseString = await client.GetStringAsync(uri);
                var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
                orders = data;
            }
            catch(Exception ex)
            {
                
            }
            return View(orders);
        }

        // Filter orders list
        [HttpPost]
        public async Task<IActionResult> Index(DateTime dateStart, DateTime dateEnd)
        {
            ViewBag.dateStart = dateStart.ToString("yyyy-MM-dd");
            ViewBag.dateEnd = dateEnd.ToString("yyyy-MM-dd");
            var orders = new List<OrderModel>();
            var uri = @"https://localhost:7063/api/Orders";
            HttpClient client = new HttpClient();

            var responseString = await client.GetStringAsync(uri);
            var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);
            if(data != null)
            {
                orders = data.Where(e => e.Date > dateStart && e.Date < dateEnd).ToList();
            }            
            return View(orders);
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
    }
}
