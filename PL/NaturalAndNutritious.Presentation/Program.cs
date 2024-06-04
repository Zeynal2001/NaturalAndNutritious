using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Business.Services;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;


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
builder.Services.AddScoped<IEmailService, EmailService>(_ => new EmailService(new()
{
    Name = builder.Configuration["EmailCredentials:Gmail:Name"],
    From = builder.Configuration["EmailCredentials:Gmail:From"],
    Host = builder.Configuration["EmailCredentials:Gmail:Host"],
    Port = Convert.ToInt32(builder.Configuration["EmailCredentials:Gmail:Port"] ?? "465"),
    User = builder.Configuration["EmailCredentials:Gmail:User"],
    Pass = builder.Configuration["EmailCredentials:Gmail:Pass"]
}));
builder.Services.RegisterServices();

// Create a LoggerFactory and add it to the ILoggerFactory
builder.Logging.AddConsole();
builder.Logging.AddFile("Logs/logs-{Date}.txt");

var app = builder.Build();

// Create an ILogger using ILoggerFactory
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();


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

logger.LogInformation("Launching the app...");

app.Run();