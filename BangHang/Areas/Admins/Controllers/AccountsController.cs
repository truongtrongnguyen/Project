
using BangHang.Models;
using BangHang.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace BangHang.Areas.Admin.Controllers
{
    [Area("Admins")]
    [Route("/Account/[action]")]
    [Authorize(Roles = "AdminManager")]
    public class AccountsController : Controller
    {
        private readonly AppDbContext _context;
        private const string folderImage = "UsersImage";
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountsController(AppDbContext context
                                    )
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index([FromQuery(Name = "p")] int currentPage, int pageSize)
        {
            IEnumerable<AppUser> users = _context.Users.ToList();
            int countPage = users.Count();
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

            var qr = users.Skip((currentPage - 1) * pageSize).Take(pageSize);

            ViewBag.UsersIndex = (currentPage - 1) * pageSize;
            ViewBag.pagingModel = pagingModel;

            ViewBag.folderImage = folderImage;
            return View(qr.ToList());
        }


        [HttpGet]
        public async Task<IActionResult> Statistical()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetStatisticalDay( string dateFrom, string dateTo)
        {
            DateTime dateStart = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dateEnd = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
          
            var query = from order in _context.Orders
                        join orderDetails in _context.OrderDetails
                        on order.Id equals orderDetails.OrderID
                        join product in _context.Products
                        on orderDetails.ProductID equals product.Id
                        where order.DateCreate >= dateStart && order.DateCreate <= dateEnd
                        orderby order.DateCreate
                        select new
                        {
                            CreateDate = order.DateCreate,
                            Quantity = orderDetails.Quantity,
                            Price = orderDetails.Price,
                            OriginalPrice = product.originalPrice
                        };

            var result = query.ToList().GroupBy(x => x.CreateDate.GetValueOrDefault().ToString("dd/MM/yyyy"))
                            .Select(x => new
                            {   
                                Date = x.Key.ToString(),
                                TotalBuy = x.Sum(xx => xx.OriginalPrice * xx.Quantity),
                                TotalSel = x.Sum(xx => xx.Price * xx.Quantity)
                            })
                            .Select(x => new
                            {
                                Date = x.Date,
                                LoiNhuan = x.TotalSel - x.TotalBuy,
                                DoanhThu = x.TotalSel
                            })
                            ;
            var Amount_Buy = result.Sum(x => x.DoanhThu);
            var Amount_Sel = result.Sum(x => x.LoiNhuan);
            return Json(new { result = result, Amount_Buy = Amount_Buy, Amount_Sel = Amount_Sel });
        }

        [HttpGet]
        public IActionResult GetStatisticalMonth(string dateFrom, string dateTo)
        {
            DateTime dateStart = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dateEnd = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var query = from order in _context.Orders
                        join orderDetails in _context.OrderDetails
                        on order.Id equals orderDetails.OrderID
                        join product in _context.Products
                        on orderDetails.ProductID equals product.Id
                        where (order.DateCreate.Value.Month >= dateStart.Month && order.DateCreate.Value.Month <= dateEnd.Month)
                        && (order.DateCreate.Value.Year >= dateStart.Year && order.DateCreate.Value.Year <= dateEnd.Year)
                        orderby order.DateCreate
                        select new
                        {
                            CreateDate = order.DateCreate,
                            Quantity = orderDetails.Quantity,
                            Price = orderDetails.Price,
                            OriginalPrice = product.originalPrice
                        };

            var result = query.ToList().GroupBy(x => x.CreateDate.GetValueOrDefault().ToString("MM/yyyy"))
                            .Select(x => new
                            {
                                Date = x.Key.ToString(),
                                TotalBuy = x.Sum(xx => xx.OriginalPrice * xx.Quantity),
                                TotalSel = x.Sum(xx => xx.Price * xx.Quantity)
                            })
                            .Select(x => new
                            {
                                Date = x.Date,
                                LoiNhuan = x.TotalSel - x.TotalBuy,
                                DoanhThu = x.TotalSel
                            })
                            ;

            var Amount_Buy = result.Sum(x => x.DoanhThu);
            var Amount_Sel = result.Sum(x => x.LoiNhuan);
            return Json(new { result = result, Amount_Buy = Amount_Buy, Amount_Sel = Amount_Sel });
        }

        [HttpGet]
        public IActionResult GetStatisticalYear(string dateFrom, string dateTo)
        {
            DateTime dateStart = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime dateEnd = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var query = from order in _context.Orders
                        join orderDetails in _context.OrderDetails
                        on order.Id equals orderDetails.OrderID
                        join product in _context.Products
                        on orderDetails.ProductID equals product.Id
                        where order.DateCreate.Value.Year >= dateStart.Year && order.DateCreate.Value.Year <= dateEnd.Year
                        orderby order.DateCreate
                        select new
                        {
                            CreateDate = order.DateCreate,
                            Quantity = orderDetails.Quantity,
                            Price = orderDetails.Price,
                            OriginalPrice = product.originalPrice
                        };

            var result = query.ToList().GroupBy(x => x.CreateDate.GetValueOrDefault().ToString("yyyy"))
                            .Select(x => new
                            {
                                Date = x.Key.ToString(),
                                TotalBuy = x.Sum(xx => xx.OriginalPrice * xx.Quantity),
                                TotalSel = x.Sum(xx => xx.Price * xx.Quantity),
                            })
                            .Select(x => new
                            {
                                Date = x.Date,
                                LoiNhuan = x.TotalSel - x.TotalBuy,
                                DoanhThu = x.TotalSel
                            })
                            ;

            var Amount_Buy = result.Sum(x => x.DoanhThu);
            var Amount_Sel = result.Sum(x => x.LoiNhuan);
            return Json(new { result = result, Amount_Buy = Amount_Buy, Amount_Sel = Amount_Sel });
        }
    }
}
