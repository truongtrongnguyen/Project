using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class CategoryProduct : ViewComponent
    {
        public IViewComponentResult Invoke(List<CategoryPro> categoryPro)
        {
            
            return View(categoryPro);
        }
    }
}