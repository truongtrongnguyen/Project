using BangHang.Models.Blog;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class Blog : ViewComponent
    {
        public IViewComponentResult Invoke(List<Post> blog)
        {
            return View(blog);
        }
    }
}
