using BangHang.Models;
using BangHang.Models.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDistributedMemoryCache();  

builder.Services.AddSession(cfg =>
{  // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "truongtrongnguyen";   
    cfg.IdleTimeout = new TimeSpan(0, 30, 0);   
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<CartService>();

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddRazorPages();


var connection = "";
builder.Services.AddDbContext<AppDbContext>(options =>
{
     connection = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    options.UseSqlServer(connection);
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSetting");
builder.Services.Configure<MailSetting>(mailSetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/users/login/";
    options.LogoutPath = "/users/register/";
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


app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
        RequestPath = "/Contents"
});

app.UseRouting();

app.UseAuthentication();  
app.UseAuthorization();   

app.UseEndpoints(endpoint =>
{
    endpoint.MapRazorPages();

});

app.MapControllerRoute(
    name: "default",
    pattern: "/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
