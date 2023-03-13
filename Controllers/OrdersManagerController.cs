using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OrdersManager.Models;
using System.Text;

namespace OrdersManager.Controllers
{
    public class OrdersManagerController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {           
                var uri = @"https://localhost:7063/api/Orders";
                HttpClient client = new HttpClient();
                var responseString = await client.GetStringAsync(uri);
                var data = JsonConvert.DeserializeObject<List<OrderModel>>(responseString);

                return View(data);

            }
            catch(Exception ex)
            {
                
            }
            return View();
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
