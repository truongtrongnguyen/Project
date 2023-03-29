using BangHang.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace App.Components
{
    [ViewComponent]
    public class MenuTop : ViewComponent
    { 
        public IViewComponentResult Invoke(List<Category> categories)
        {
            return View(categories);
        }
    }
}