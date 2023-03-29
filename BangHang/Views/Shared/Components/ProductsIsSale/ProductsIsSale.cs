using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class ProductsIsSale : ViewComponent
    {
        public IViewComponentResult Invoke(List<Products> productsIsSale)
        {
            return View(productsIsSale);
        }
    }
}
