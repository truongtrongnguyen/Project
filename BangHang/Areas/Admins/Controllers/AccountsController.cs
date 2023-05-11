
using BangHang.Models;
using BangHang.Models.OderProduct;
using BangHang.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;

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
        public async Task<IActionResult> GetStatistical(string dateFrom, string dateTo)
        {
            var query = from order in _context.Orders
                        join orderDetails in _context.OrderDetails
                        on order.Id equals orderDetails.OrderID
                        join product in _context.Products
                        on orderDetails.ProductID equals product.Id
                        select new
                        {
                            CreateDate = order.DateCreate,
                            Quantity = orderDetails.Quantity,
                            Price = orderDetails.Price,
                            OriginalPrice = product.originalPrice
                        };


            if (!string.IsNullOrEmpty(dateFrom))
            {
                var dateStart = DateTime.ParseExact(dateFrom, "dd/mm/yyyy", null);
                query = query.Where(x => x.CreateDate >= dateStart);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                var dateEnd = DateTime.ParseExact(dateTo, "dd/mm/yyyy", null);
                query = query.Where(x => x.CreateDate <= dateEnd);
            }

            var result = query.ToList().GroupBy(x => x.CreateDate.GetValueOrDefault().Day)
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

            return Json(new {result  = result});
        }



    }
}
