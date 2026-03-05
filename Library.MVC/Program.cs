using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Add services
// --------------------
builder.Services.AddControllersWithViews();

// Database (SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=library.db"));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// --------------------
// Seed Roles + Admin
// --------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}

// --------------------
// Configure Middleware
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();