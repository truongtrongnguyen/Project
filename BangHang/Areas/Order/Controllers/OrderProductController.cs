using BangHang.Models;
using BangHang.Models.OderProduct;
using BangHang.Models.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BangHang.Areas.Models.SendMail;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<AppUser> _userManager;

        public OrderProductController(AppDbContext context,
                                        CartService cartService,
                                        IEmailSender emialSender,
                                        IOptions<MailSetting> mailSetting,
                                        UserManager<AppUser> userManager
                                        )
        {
            _context = context;
            _cartService = cartService;
            _emailSender = emialSender;
            _mailSetting = mailSetting.Value;
            _userManager = userManager;
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
            if (order != null)
            {
                return View(order);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            decimal total = 0;

            var cartItems = _cartService.GetCartItems();
            List<CartItemModel> cartItemsList = new List<CartItemModel>();
            foreach (var item in cartItems)
            {
                var cart = _context.Products?
                            .Include(p => p.ProductPhoto)
                            .FirstOrDefault(p => p.Id == item.ProductId);

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

                // Save Session
                item.NameProduct = cart.Title;
                item.Price = cart.Price;
                item.PriceSale = cart.PriceSale;
                item.Color = cart.Color;
                item.TypeProduct = cart.TypeProduct;
                item.ImageAvata = cart.ProductPhoto.Select(x => x.Name).First();
            }
            _cartService.SaveCartSession(cartItems);

            var order = new Order();
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                order.CustomerName = user.FullName;
                order.Phone = user.PhoneNumber;
                order.Address = $"{user.DetailsAddress}, {user.Ward}, {user.District}, {user.City}";
                order.Email = user.Email;
            }

            ViewBag.TotalAmount = total;
            ViewBag.cartItems = cartItemsList;
            ViewBag.date = cartItemsList;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order orderModel)
        {
            var cartItems = _cartService.GetCartItems();

            if (ModelState.IsValid && cartItems?.Count > 0)
            {
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

                // creata orderdetails
                var orderDetails = new List<OrderDetail>();

                foreach (var item in cartItems)
                {
                    orderDetails.Add(new OrderDetail()
                    {
                        Order = order,
                        ProductID = item.ProductId,
                        Price = item.Price,
                        PriceSale = item.PriceSale,
                        Quantity = item.quantity,
                        NameProduct = item.NameProduct,
                        ImageAvata = item.ImageAvata,
                        TypeProduct = item.TypeProduct,
                        Color = item.Color,
                        CreateDate = order.DateCreate,
                        
                    });
                }

                await _context.OrderDetails.AddRangeAsync(orderDetails);

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user != null)
                {
                    user.OrdersCount += 1;
                    order.UserId = user.Id;
                }

                await _context.SaveChangesAsync();

                await _emailSender.SendEmailAsync(_mailSetting.Email, $"Đơn hàng từ {order.CustomerName}", SendMailAdmin.SendEMailAdmin(order, orderDetails));
                await _emailSender.SendEmailAsync(order.Email, $"Bạm vừa đặt hàng thành công", SendMailAdmin.SendEMailClient(order, orderDetails));

                _cartService.CLearCart();

                return RedirectToAction("OrderSuccess", "Menu");
            }

            return await Create();
        }

        [HttpPost]
        public IActionResult UpdatePayment(int id, int trangthai)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Payment = trangthai;
                _context.SaveChanges();
                return Json(new { success = true, payment = order.Payment });
            }
            return Json(new { success = false, payment = order?.Payment });
        }
    }
}
