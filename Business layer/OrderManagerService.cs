using OrdersManager.Models;

namespace OrdersManager.Business_layer
{
    public class OrderManagerService : IOrderManagerService
    {
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
    }
}
