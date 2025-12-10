using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShiftManagement.Data;
using ShiftManagement.Models;
using ShiftManagement.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=shiftmanagement.db"));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add services
builder.Services.AddScoped<ExportService>();
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddSession();

// Configure antiforgery (temporarily disabled for testing)
// builder.Services.AddAntiforgery(options =>
// {
//     options.HeaderName = "X-CSRF-TOKEN";
//     options.Cookie.Name = "X-CSRF-COOKIE";
//     options.Cookie.HttpOnly = true;
//     options.Cookie.SecurePolicy = CookieSecurePolicy.None;
//     options.Cookie.SameSite = SameSiteMode.Lax;
// });

var app = builder.Build();

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Starting database seeding...");
        await SeedData(context, userManager, roleManager, logger);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration or seeding.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) // Исправлено использование IsDevelopment
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Добавить поддержку статических файлов
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "public,max-age=3600";
    }
});

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger logger)
{
    logger.LogInformation("Checking for Admin role...");
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        logger.LogInformation("Creating Admin role...");
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    logger.LogInformation("Checking for Employee role...");
    if (!await roleManager.RoleExistsAsync("Employee"))
    {
        logger.LogInformation("Creating Employee role...");
        await roleManager.CreateAsync(new IdentityRole("Employee"));
    }

        logger.LogInformation("Checking for admin user...");
    var adminUser = await userManager.FindByNameAsync("admin@store.com");
    if (adminUser == null)
    {
        logger.LogInformation("Creating admin user...");
        adminUser = new ApplicationUser
        {
            UserName = "admin@store.com",
            Email = "admin@store.com"
        };
        var result = await userManager.CreateAsync(adminUser, "AdminPass123!");
        if (result.Succeeded)
        {
            logger.LogInformation("Admin user created successfully");
            await userManager.AddToRoleAsync(adminUser, "Admin");
            logger.LogInformation("Admin role assigned");

            // Create admin employee
            logger.LogInformation("Creating admin employee...");
            var adminEmployee = new Employee
            {
                FirstName = "Admin",
                LastName = "User",
                Position = "Administrator",
                Email = "admin@store.com",
                HireDate = DateTime.UtcNow,
                UserId = adminUser.Id
            };
            context.Employees.Add(adminEmployee);
            await context.SaveChangesAsync();
            logger.LogInformation("Admin employee created");
        }
        else
        {
            logger.LogError("Failed to create admin user: {errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        logger.LogInformation("Admin user already exists");
        
        // Check if admin user has Admin role
        var isAdmin = await userManager.IsInRoleAsync(adminUser, "Admin");
        logger.LogInformation($"Admin user role check: {isAdmin}");
        
        // Check if admin user has associated employee
        var adminEmployee = await context.Employees.FirstOrDefaultAsync(e => e.UserId == adminUser.Id);
        if (adminEmployee == null)
        {
            logger.LogInformation("Creating missing admin employee...");
            adminEmployee = new Employee
            {
                FirstName = "Admin",
                LastName = "User", 
                Position = "Administrator",
                Email = "admin@store.com",
                HireDate = DateTime.UtcNow,
                UserId = adminUser.Id
            };
            context.Employees.Add(adminEmployee);
            await context.SaveChangesAsync();
            logger.LogInformation("Admin employee created");
        }
        else
        {
            logger.LogInformation($"Admin employee exists: {adminEmployee.FirstName} {adminEmployee.LastName}");
        }
    }
}
