using System;
using Microsoft.AspNetCore.Mvc;
using OrdersManager.Models;

namespace OrdersManager.Components
{
    [ViewComponent]
    public class DataTransferView : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(EditCreatePageModel model)
        {
            return View(model);
        }
    }
}

