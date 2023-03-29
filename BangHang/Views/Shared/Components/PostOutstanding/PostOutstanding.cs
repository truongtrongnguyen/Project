using BangHang.Models.Blog;
using Microsoft.AspNetCore.Mvc;

namespace App.Components
{
    [ViewComponent]
    public class PostOutstanding : ViewComponent
    {
        public IViewComponentResult Invoke(Post postOutstanding)
        {
            return View(postOutstanding);
        }
    }
}
