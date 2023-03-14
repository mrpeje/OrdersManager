using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrdersManager.Models
{
    public class IndexPageModel
    {
        public IndexPageModel(List<OrderModel> orders, List<SelectListItem> providerList, List<SelectListItem> numberList)
        {
            Orders = orders;
            ProviderList = providerList;
            NumberList = numberList;
        }
        public List<OrderModel> Orders { get; set; }
        public List<SelectListItem> ProviderList { get; set; }
        public List<SelectListItem> NumberList { get; set; }

    }
}
