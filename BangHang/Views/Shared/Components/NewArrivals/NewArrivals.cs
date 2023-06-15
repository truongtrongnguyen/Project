using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class NewArrivals : ViewComponent
    {
        public IViewComponentResult Invoke(List<Products>? Products)
        {
            return View(Products);
        }
    }
}