using NailMVC.Data;
using Microsoft.EntityFrameworkCore;
using NailMVC.Services;
using NailMVC.Filters;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<NailDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // 🕒 Tự động đăng xuất sau 10 phút không hoạt động
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHostedService<ScheduledReportService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ActivityLogger>();
builder.Services.AddScoped<ActionLoggingFilter>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<ActionLoggingFilter>(); // Không cần Filters. ở đây
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
