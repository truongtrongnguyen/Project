using BangHang.Models;
using BangHang.Models.OderProduct;
using BangHang.Models.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BangHang.Areas.Models.SendMail;
using Microsoft.Extensions.Options;

namespace BangHang.Areas.OrderProducts.Controllers
{
    [Area("Order")]
    [Route("/order/orderproduct/[action]/{id?}")]
    public class OrderProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CartService _cartService;
        private readonly IEmailSender _emailSender;
        private readonly MailSetting _mailSetting;

        public OrderProductController(AppDbContext context, CartService cartService, IEmailSender emialSender, IOptions<MailSetting> mailSetting)
        {
            _context = context;
            _cartService = cartService;
            _emailSender = emialSender;
            _mailSetting = mailSetting.Value;
        }

         [HttpGet]
        [HttpPost]
        public IActionResult Index([FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            IEnumerable<Order> orders = _context.Orders.OrderByDescending(c => c.DateCreate);

            int countPage = orders.Count();
            pageSize = 5;
            int pageNumber = (int)Math.Ceiling((double)countPage / pageSize);
            if (currentPage > countPage) currentPage = countPage;
            if (currentPage < 1) currentPage = 1;
            var pagingModel = new PagingModel()
            {
                countPages = pageNumber,
                currentPage = currentPage,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    pageSize = pageSize
                })
            };

            var qr = orders.Skip((currentPage - 1) * pageSize).Take(pageSize);

            ViewBag.OrderIndex = (currentPage - 1) * pageSize;
            ViewBag.pagingModel = pagingModel;

            return View(qr.ToList());
        }

        [HttpGet]
        public IActionResult OrderDetail(int id)
        {
            var order = _context.Orders
                        .Where(c => c.Id == id)
                        .Include(c => c.OrderDetails)
                        .FirstOrDefault();
            if(order != null)
            {
                return View(order);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            // Category Post
            var categoryTop = _context.Categories.ToList();
            ViewBag.categoryTop = categoryTop;

            decimal total = 0;

            var cartItems = _cartService.GetCartItems();
            List<CartItemModel> cartItemsList = new List<CartItemModel>();
            foreach (var item in cartItems)
            {
                var cart = _context.Products?
                            .Include(p => p.ProductPhoto)?
                            .Include(p => p.ProductCategory)
                            .ThenInclude(pc => pc.Category)
                            .Where(p => p.Id == item.ProductId)
                            .FirstOrDefault();

                cartItemsList.Add(new CartItemModel()
                {
                    Product = cart,
                    quantity = item.quantity
                });

                if (cart?.PriceSale > 0)
                {
                    total += cart.PriceSale.GetValueOrDefault() * item.quantity.GetValueOrDefault();
                }
                else
                {
                    total += cart.Price.GetValueOrDefault() * item.quantity.GetValueOrDefault();
                }
            }
            ViewBag.TotalAmount = total;
            ViewBag.cartItems = cartItemsList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order orderModel)
        {

            var cartItems = _cartService.GetCartItems();

            if (ModelState.IsValid && cartItems?.Count > 0)
            {
                // Do CartItems lưu trên Section chỉ lưu số lượng và Id sản phẩm nên ta phải tìm các sản phẩm đó lại convert thành list 
                // để thêm vào OrderDetail
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

                // Tạo đơn hàng
                Order order = new Order()
                {
                    CustomerName = orderModel.CustomerName,
                    Phone = orderModel.Phone,
                    Email = orderModel.Email,
                    DateCreate = DateTime.Now,
                    Address = orderModel.Address,
                    Payment = orderModel.Payment,
                    TotalAmount = orderModel.TotalAmount,
                    Code = "MH" + DateTime.Now.Year.ToString()
                            + DateTime.Now.Month.ToString()
                            + DateTime.Now.Day.ToString()
                            + DateTime.Now.Hour.ToString()
                            + DateTime.Now.Minute.ToString()
                            + DateTime.Now.Second.ToString()
                };
                await _context.SaveChangesAsync();

                // creata orderdetails
                var orderDetails = new List<OrderDetail>();
                foreach (var item in cartItemsList)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        Order = order,
                        Product = item.Product,
                        Price = item.Product.Price,
                        PriceSale = item.Product.PriceSale,
                        Quantity = item.quantity,
                        NameProduct = item.Product.Title,
                        ImageAvata = item.Product.ProductPhoto.Select(c => c.Name).FirstOrDefault(),
                        TypeProduct = item.Product.TypeProduct,
                        Color = item.Product.Color,
                        CreateDate = order.DateCreate
                    });
                }
                _context.OrderDetails.AddRange(orderDetails);
                _emailSender.SendEmailAsync(_mailSetting.Email, $"Đơn hàng từ {order.CustomerName}", SendMailAdmin.SendEMailAdmin(order, orderDetails));
                _emailSender.SendEmailAsync(order.Email, $"Bạm vừa đặt hàng thành công", SendMailAdmin.SendEMailClient(order, orderDetails));
                await _context.SaveChangesAsync();
                _cartService.CLearCart();
                return RedirectToAction("OrderSuccess", "Menu");
            }
            // Category Post
            var categoryTop = _context.Categories.ToList();
            ViewBag.categoryTop = categoryTop;
            return Create();
        }

        [HttpPost]
        public IActionResult UpdatePayment (int id, int trangthai)
        {
            var order = _context.Orders.Find(id);
            if(order != null)
            {
                order.Payment = trangthai;
                _context.SaveChanges();
                return Json(new { success = true, payment = order.Payment });
            }
            return Json(new { success = false, payment = order?.Payment });
        }

    }
}
