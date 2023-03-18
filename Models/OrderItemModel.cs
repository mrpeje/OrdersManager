using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OrdersManager.Models
{
    public class OrderItemModel
    {
        [HiddenInput(DisplayValue = false)]
        public int OrderId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public string Unit { get; set; }

        [HiddenInput(DisplayValue = false)]
        public OrderModel? Order { get; set; }
    }
}
