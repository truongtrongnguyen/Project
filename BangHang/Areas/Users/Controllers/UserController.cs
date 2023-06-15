using BangHang.Areas.Users.Models;
using BangHang.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Users.Controllers
{
    [Area("Users")]
    [Route("/users/[action]")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        public const string folderImage = "UsersImage";
        private readonly SignInManager<AppUser> _signInManager;
        private readonly string directoryFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage);
        public UserController(UserManager<AppUser> userManager,
                                AppDbContext context,
                                SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;

        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Users" });
            }

            var userModel = new UserDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Sex = user.Sex,
                BirthDay = user.BirthDay.GetValueOrDefault(),
                Image = user.Avata,
                OrderCount = user.OrdersCount,
                City = user.City,
                Distric = user.District,
                Wrad = user.Ward,
                AddressDetails = user.DetailsAddress,
                EmailConfirm = user.EmailConfirmed
            };
            ViewBag.folderImage = folderImage;
            return View(userModel);
        }

        private Task<AppUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAsync();
            var userModel = new UserDto()
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Sex = user.Sex,
                BirthDay = user.BirthDay.GetValueOrDefault(),
                Image = user.Avata,
                City = user.City,
                Distric = user.District,
                Wrad = user.Ward,
                AddressDetails = user.DetailsAddress,
            };

            ViewBag.folderImage = folderImage;

            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDto request)
        {
            if (!ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
                if (user != null)
                {
                    user.FullName = request.FullName;
                    user.Sex = request.Sex;
                    user.BirthDay = request.BirthDay;
                    user.PhoneNumber = request.PhoneNumber;
                    user.City = request.City;
                    user.District = request.Distric;
                    user.Ward = request.Wrad;
                    user.DetailsAddress = request.AddressDetails;

                    // check folder exists, if folder not exists for create new folder
                    if (!Directory.Exists(directoryFolder))
                    {
                        Directory.CreateDirectory(directoryFolder);
                    }

                    if (request.Avata != null)
                    {
                        if (!string.IsNullOrEmpty(user.Avata))
                        {
                            var deleteAvata = Path.Combine(directoryFolder, user.Avata);
                            if (!Directory.Exists(deleteAvata))
                            {
                                System.IO.File.Delete(deleteAvata);
                            }
                        }

                        var fileName = Path.GetRandomFileName() +
                                        Path.GetExtension(request.Avata.FileName);
                        var file = Path.Combine(directoryFolder, fileName);

                        using (FileStream fileStream = new FileStream(file, FileMode.Create))
                        {
                            await request.Avata.CopyToAsync(fileStream);
                        };
                        user.Avata = fileName;
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "User", new { area = "Users" });
                }
            }
            return View(request);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DetailOrderUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return RedirectToAction("Login", "User", new { area = "Users" });
            }

            var order = _context.Orders
                                .Include(x => x.OrderDetails)
                                .Where(x => x.UserId == user.Id)
                                .AsNoTracking()
                                .OrderByDescending(x => x.DateCreate)
                                .ToList();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAddress( [Bind("id")] string id,
                                                       [Bind("City")] string City,
                                                       [Bind("Distric")] string Distric,
                                                       [Bind("Ward")] string Ward,
                                                       [Bind("AddressDetails")] string AddressDetails)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(City) 
                && !string.IsNullOrEmpty(Distric) && !string.IsNullOrEmpty(Ward))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.City = City;
                    user.District = Distric;
                    user.Ward = Ward;
                    user.DetailsAddress = AddressDetails;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
         }



        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel request)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại, vui lòng kiểm tra lại");
                    return View();
                }
                var result = await _signInManager.PasswordSignInAsync(user.Email, request.Password, false, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Sai mật khẩu hoặc tài khoản không tồn tại, vui lòng kiểm tra lại");
                    return View();
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Tài khoản đã bị khóa");
                    return View();
                }
                if (result is not null)
                {
                    return RedirectToAction("Index", "Menu", new { area = "Menus" });
                }
            }

            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Menu", new { area = "Menus" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterModel request)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Emai đã tồn tại tài khoản, vui lòng sử dụng Email khác");
                    return View();
                }

                var newUser = new AppUser()
                {
                    Email = request.Email,
                    UserName = request.Email,
                    EmailConfirmed = true,
                    FullName = request.FullName,
                };
                var result = await _userManager.CreateAsync(newUser, request.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account", new { area = "Admins" });
                }
            }

            var message = string.Join(" | ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            ModelState.AddModelError("", message);
            return View(request);
        }
    }
}
