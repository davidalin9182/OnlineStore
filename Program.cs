using Microsoft.EntityFrameworkCore;
using Proiect_IR.Services;
using Proiect_IR.Data;
using Proiect_IR.Helpers;
using Proiect_IR.Interfaces;
using Proiect_IR.Repository;
using Proiect_IR.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register ProductRepository with connection string
builder.Services.AddScoped<IProductRepository>(provider =>
{
    var dbContext = provider.GetService<ApplicationDbContext>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new ProductRepository(dbContext, connectionString);
});
//builder.Services.AddScoped<IProductRepository>(provider =>
//{
//    var dbContext = provider.GetService<ApplicationDbContext>();
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    var productIndexer = provider.GetService<ProductIndexer>();
//    return new ProductRepository(dbContext, connectionString, productIndexer);
//});


builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ProductIndexer>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();





var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
{
    await Seed.SeedUsersAndRolesAsync(app);
    Seed.SeedData(app);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();
