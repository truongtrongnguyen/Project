using BangHang.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Admins.Controllers
{
    [Area("Admins")]
    [Route("/dbmanager/[action]")]
    public class DbManagerController : Controller
    {
        private readonly AppDbContext _dbcontext;
        private readonly UserManager<AppUser> _userManage;
        private readonly RoleManager<IdentityRole> _roleManage;

        public DbManagerController(AppDbContext dbcontext, UserManager<AppUser> userManage, RoleManager<IdentityRole> roleManage)
        {
            _dbcontext = dbcontext;
            _userManage = userManage;
            _roleManage = roleManage;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // Khi Delete database xong thì ta cần đăng nhập với tài khoản admin mới SeeData được nếu không nó sẽ báo lỗi
        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync()
        {
            var success = await _dbcontext.Database.EnsureDeletedAsync();
            return RedirectToAction("Index", "Menu", new {area = "Menus"});
        }


        public async Task<IActionResult> SeedDataAsync()
        {
            var useradmin = await _userManage.FindByEmailAsync("AdminManager@gmail.com"); 
            if (useradmin == null)
            {
                useradmin = new AppUser()
                {
                    FullName = "Admin Manager",
                    Email = "AdminManager@gmail.com",
                    EmailConfirmed = true,
                    UserName = "AdminManager@gmail.com"
                };

                var newUser = await _userManage.CreateAsync(useradmin, "Admin@123");

                if (newUser.Succeeded)
                {
                    var roleExist = await _roleManage.FindByNameAsync("AdminManager");

                    if (roleExist == null)
                    {
                        var role = new IdentityRole("AdminManager");
                        await _roleManage.CreateAsync(role);
                    }

                    await _userManage.AddToRoleAsync(useradmin, "AdminManager");
                }
            }
            return RedirectToAction("Login", "User", new { area = "Users"});
        }


        [HttpPost]
        public async Task<IActionResult> Migrate()
        {
            await _dbcontext.Database.MigrateAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
