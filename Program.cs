using BookLibraryApp.Models.Entities;
using BookLibraryApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookLibraryApp;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------
// Identity Configuration
// ---------------------------------------------------------------------
// 1. Add Identity services (Must be before builder.Services.AddControllersWithViews())
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() //THIS LINE enables role management
    .AddEntityFrameworkStores<LibraryDbContext>();

// 2. Add Razor Pages support for Identity UI views
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

//register database
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryConnection")));

//register book service 
builder.Services.AddScoped<IBookService, BookService>();

// Register the Loan service
builder.Services.AddScoped<ILoanService, LoanService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


/// Must come AFTER UseRouting() and BEFORE UseEndpoints()
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


// For standard MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// For Identity Razor Pages (e.g., /Identity/Account/Login)
app.MapRazorPages(); // <-- Required for Identity UI



//   this block to call the role initializer on startup
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        await BookLibraryApp.Areas.Identity.Data.DbInitializer.InitializeAsync(serviceProvider);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database with roles.");
    }
}
app.Run();
