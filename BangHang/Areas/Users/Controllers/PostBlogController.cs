using BangHang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Users.Controllers
{
    [Area("Users")]
    [Route("/postblog/[action]")]
    public class PostBlogController : Controller
    {
        private readonly AppDbContext _context;
        public PostBlogController(AppDbContext contex)
        {
            _context = contex;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var post = _context.Posts
                             .Include(x => x.PostCategory)
                            .ThenInclude(xx => xx.Category)
                            .AsNoTracking()
                            .OrderByDescending(x => x.DateCreate).ToList();

            var category = _context.Categories.ToList();
            ViewBag.Category = category;
            return View(post);
        }

        [HttpGet]
        public IActionResult PostBlogDetail(int id)
        {
            var post = _context.Posts
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);

            return View(post);
        }

        [HttpGet]
        public IActionResult PostBlogCategory(int id)
        {
            var category = _context.Categories
                            .Include(x => x.CategoryChildrent)
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == id);

            var qr = _context.Posts
                         .Include(x => x.PostCategory)   
                         .ThenInclude(xx => xx.Category)
                           .AsQueryable();

            var posts = from post1 in qr
                        where post1.PostCategory.Where(xx => xx.CategoryID == category.Id.GetValueOrDefault()).Any()
                        select post1;
            ViewBag.Category = category;
            return View(posts.ToList());
        }
    }
}
