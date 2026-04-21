using Microsoft.EntityFrameworkCore;
using WebStore.Models;
using WebStore.Service;
using QuestPDF;                
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Cấu hình DbContext với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm dịch vụ cache bộ nhớ tạm dùng cho Session
builder.Services.AddDistributedMemoryCache();

// Thêm dịch vụ Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // Thời gian sống của session
    options.Cookie.HttpOnly = true;                  // Cookie chỉ cho HTTP truy cập (bảo mật)
    options.Cookie.IsEssential = true;               // Cookie thiết yếu
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kích hoạt session trước UseAuthorization hoặc MapControllerRoute
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "HomeStore", action = "Index" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
