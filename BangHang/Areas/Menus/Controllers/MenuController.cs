using BangHang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BangHang.Models.Product;
using System.Linq;
using BangHang.Models.Services;

namespace BangHang.Areas.Menus.Controllers
{
    [Area("Menus")]
    [Route("/Menu/[action]")]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CartService cartService;

        public MenuController(AppDbContext context, CartService cartservice)
        {
            _context = context;
            cartService = cartservice;
        }
        [HttpGet]
        public IActionResult ShoppingGuide()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Post Sale IsActive
            var PostSale = _context.Posts
                                .Where(c => c.IsActive == true)
                                .Take(1).FirstOrDefault();
            ViewBag.PostSale = PostSale;

            // Category Product
            var categoryPro = _context.CategoriesPro
                            .Where(c => c.IsActive == true)
                           .Take(6)
                            .ToList();
            ViewBag.CategoryPro = categoryPro;

            var Products = _context.Products
                                    .Include(p => p.ProductPhoto)
                                    .Include(p => p.ProductCategory)
                                    .ThenInclude(pc => pc.Category)
                                     .Where(c => c.IsActive == true)
                                      .Take(20)
                                      .OrderByDescending(x => x.DateCreate)
                                    .ToList();


            ViewBag.Products = Products;

            // Post nổi bậc
            var postOutstanding = _context.Posts.Where(p => p.IsHot == true).FirstOrDefault();
            ViewBag.postOutstanding = postOutstanding;

            // Products Sale
            var productIsSale = _context.Products
                                 .Include(p => p.ProductPhoto)
                                  .Where(p => p.IsSale)
                                  .Take(20)
                                  .ToList();
            ViewBag.productIsSale = productIsSale;

            // Blog
            var blog = _context.Posts
                    .Where(p => p.IsActive == false && p.IsHot == false)
                    
                     .OrderByDescending(x => x.DateCreate)
					 .Take(6)
					.ToList();
            ViewBag.Blog = blog;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Categories(string? slug)
        {

            // Category Product
            var categoryPro = _context.CategoriesPro
                                .ToList();
            ViewBag.CategoriesPro = categoryPro;

            // CategoryProduct --> Slug
            CategoryPro categoryslug = null;
            if (!String.IsNullOrEmpty(slug))
            {
                categoryslug = await _context.CategoriesPro
                                    .Where(c => c.Slug == slug)
                                    .FirstOrDefaultAsync();
            }
            else
            {
                categoryslug = await _context.CategoriesPro
                                    .FirstAsync();
            }

            var products = _context.Products
                            .Include(p => p.ProductPhoto)
                            .Include(p => p.ProductCategory)
                            .ToList()
                            .Where(p => p.ProductCategory.Select(c => c.CategoryID).Contains(categoryslug.Id)).ToList();
            ViewBag.products = products;
            return View(categoryslug);
        }

        [HttpGet]
        public async Task<IActionResult> ProductSingle(int id)
        {
            // Category Post
            var categoryTop = _context.Categories.ToList();
            ViewBag.categoryTop = categoryTop;

            if(id != null)
            {
                var product = _context.Products
                            .Include(p => p.ProductPhoto)
                            .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                            .Where(p => p.Id == id)
                            .FirstOrDefault();
                if (product != null)
                {
                    return View(product);
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShoppingCart()
        {

            var cartItems = cartService.GetCartItems();
            List<CartItemModel> cartItemsList = new List<CartItemModel>();
            foreach (var item in cartItems)
            {
                var cart = _context.Products
                            .Include(p => p.ProductPhoto)
                            .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                            .Where(p => p.Id == item.ProductId)
                            .FirstOrDefault();
                cartItemsList.Add(new CartItemModel()
                {
                    Product = cart,
                    quantity = item.quantity
                });
            }



            return View(cartItemsList);
        }


        [HttpPost]
        public IActionResult AddCart(int productid, int quantity)
        {
            var product = _context.Products
               .FirstOrDefault(p => p.Id == productid);

            if (product == null) return NotFound();

            List<CartItem> cart = cartService.GetCartItems();
            var cartitem = cart.Find(c => c.ProductId == productid);   // Tìm xem sản phẩm có tồn tại trong giỏ hàng hay chưa
            int count = 0;
            if (cartitem != null)
            {
                cartitem.quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem()
                {
                    ProductId = product.Id,
                    quantity = quantity
                });
            }
            count = cart.Count();
             cartService.SaveCartSession(cart);
            return Json(new { success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công", count = count });
        }

        [HttpPost]
        public IActionResult RemoveCart(int productid)
        {
            int count = 0;
            var cartItems = cartService.GetCartItems();
            if (cartItems != null)
            {
                var cart = cartItems.Find(c => c.ProductId == productid);
                if(cart != null)
                {
                    cartItems.Remove(cart);
                    cartService.SaveCartSession(cartItems);
                     count = cartItems.Count();
                    return Json(new { success = true, msg = "Xóa sản phẩm thành công", count = count });
                }

            }
            return Json(new { success = false, msg = "Xóa sản phẩm không thành công", count = count });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productid, int quantity)
        {
            var cartItems = cartService.GetCartItems();
            if(cartItems != null)
            {
                var cart = cartItems.Find(c => c.ProductId == productid);
                if(cart != null)
                {
                    cart.quantity = quantity;
                    cartService.SaveCartSession(cartItems);
                    return Json(new { success = true, msg = "Cập nhật giỏ hàng thành công" });
                }
            }
            return Json(new { success = false, msg = "Lỗi không cập nhật được giỏ hàng!!!" });
        }

        [HttpGet]
        public IActionResult OrderSuccess()
        {
            // Category Post
            var categoryTop = _context.Categories.ToList();
            ViewBag.categoryTop = categoryTop;

            return View();
        }
    }
}
