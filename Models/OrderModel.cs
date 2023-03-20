using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace OrdersManager.Models
{
    public class OrderModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int ProviderId { get; set; }

    }
}
