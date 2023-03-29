using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class NewArrivals : ViewComponent
    {
        public class NewArrivalsData
        {
            public List<CategoryPro>? CategoryPro { get; set; }
            public List<Products>? Products { get; set; }
        }
        public IViewComponentResult Invoke(NewArrivalsData newArrivalsData)
        {
            return View(newArrivalsData);
        }
    }
}