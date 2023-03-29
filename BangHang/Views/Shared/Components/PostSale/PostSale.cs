using BangHang.Models.Blog;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class PostSale : ViewComponent
    {
        public IViewComponentResult Invoke(Post post)
        {
            return View(post);
        }
    }
}
