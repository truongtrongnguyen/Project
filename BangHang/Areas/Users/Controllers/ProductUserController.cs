using BangHang.Models;
using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Users.Controllers
{
    [Area("Users")]
    [Route("/ProductUser/[action]")]
    public class ProductUserController : Controller
    {
        private readonly AppDbContext _context;
        public ProductUserController(AppDbContext contex)
        {
            _context = contex;
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
            var categoryPro = _context.CategoriesPro.ToList();
            ViewBag.CategoryPro = categoryPro;
            return View(products);
        }
    }
}
