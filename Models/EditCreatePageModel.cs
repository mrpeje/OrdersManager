﻿


namespace OrdersManager.Models
{
    public class EditCreatePageModel
    {
        public EditCreatePageModel()
        {

        }
        public EditCreatePageModel(OrderModel order, List<OrderItemModel> items)
        {
            Order = order;
            OrderItems = items;
        }
        public OrderModel Order { get; set; }

        public List<OrderItemModel>? OrderItems { get; set; }
        public OrderItemModel NewOrderItem { get; set; }     
    }
}
