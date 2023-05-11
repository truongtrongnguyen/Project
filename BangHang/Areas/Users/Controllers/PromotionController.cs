using BangHang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Users.Controllers
{
    [Area("Users")]
    [Route("/promotion/[action]")]
    public class PromotionController : Controller
    {
        private readonly AppDbContext _context;

        public PromotionController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products
                                .Include(x => x.ProductCategory)
                                .ThenInclude(xx => xx.Category)
                                .Include(x => x.ProductPhoto)

                                .Where(x => x.IsSale == true)
                                .OrderByDescending(x => x.DateCreate)
                                .AsNoTracking()
                                .ToList();
            return View(products);
        }
    }
}
