using BangHang.Models;
using BangHang.Models.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// ĐĂNG KÝ SỬ DỤNG SESSTION CHO CHỨC NĂNG GIỎ HÀNG
builder.Services.AddDistributedMemoryCache();   // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
builder.Services.AddSession(cfg =>
{  // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "truongtrongnguyen";      // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);   // Thời gian tồn tại của Session
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


// ĐĂNG KÝ DỊCH VỤ SỬ DỤNG SESSION
builder.Services.AddTransient<CartService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// THÊM TRANG RAZOR PAGE
builder.Services.AddRazorPages();

// ĐĂNG KÝ KẾT NỐI SERVER
var connection = "";
builder.Services.AddDbContext<AppDbContext>(options =>
{
     connection = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    options.UseSqlServer(connection);
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


// DĂNG KÝ DỊCH VỤ GỞI MAIL
builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSetting");
builder.Services.Configure<MailSetting>(mailSetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/users/login/";
    options.LogoutPath = "/users/register/";
    options.AccessDeniedPath = "/khongduoctruycap";// Đường dẫn tới trang khi User bị cấm truy cập
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("ViewAdminManager", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("AdminManager");
    }
    );
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();




//CẤU HÌNH ĐƯỜNG DẪN LƯU FILE 
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
        RequestPath = "/Contents"
});

app.UseRouting();

app.UseAuthentication();    // Xác định danh tính
app.UseAuthorization();     // Xác thực quyền truy cập

app.UseEndpoints(endpoint =>
{
    endpoint.MapRazorPages();

});

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
