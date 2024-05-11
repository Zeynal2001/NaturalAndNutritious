using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Business.Services;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories;
using NaturalAndNutritious.Data.Repositories.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<AppUser, IdentityRole>(op =>
{
    //op.Password.RequireUppercase = false;
    //op.Password.RequireNonAlphanumeric = false;
})
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

//-------------------------------------------------------------
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Auth/Login";
    opt.AccessDeniedPath = "/Auth/Login";
});

builder.Services.AddAuthentication()
    .AddCookie("AdminAuth", opt => //burada yeni þema yaratdýq
    {
        opt.LoginPath = "/admin_panel/Auth/Login";
        opt.AccessDeniedPath = "/admin_panel/Auth/Login";
    });
//-------------------------------------------------------------

builder.Services.AddSession();
builder.Services.AddScoped<IStorageService, LocalStorageService>();
//Services.AddSingleton<IStorageService, LocalStorageService>();
builder.Services.RegisterServices();


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
app.UseAuthentication(); //login, register ve logout emeliyyatlari ucun
app.UseAuthorization(); //user veya admin kimi rollar ucun
app.UseSession();

app.MapControllerRoute(name: "area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();