using BangHang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BangHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var category = _context.Categories.ToList();

            //var product = _context.Products
            //    .Include(p => p.ProductPhoto)
            //    .Where(p => p.IsHome == true && p.IsSale == true).FirstOrDefault();
            //ViewBag.ProductHomeSale = product;
            //return View(category);
            return RedirectToAction("Index", "Menu", new { area = "Menus" });
        }

        public IActionResult ProductHomeSale()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}