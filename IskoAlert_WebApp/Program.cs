using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using IskoAlert_WebApp.Services.Implementations;
using IskoAlert_WebApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICredibilityAnalyzerService, CredibilityAnalyzerService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

// l
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Authentication services
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        // Name of the cookie stored in the browser
        config.Cookie.Name = "UserLoginCookie";

        // Redirect path for unauthorized access
        config.LoginPath = "/Account/Login";
    });

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    
    context.Database.Migrate();


    if (!context.Users.Any())
    {
      
        context.Users.Add(new User(
            idNumber: "2021-00001-MN-0",
            webmail: "admin@iskolarngbayan.pup.edu.ph",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("TestPassword123"),
            name: "PUP Admin",
            role: UserRole.Admin
        ));

        context.SaveChanges();
    }
}

app.UseSession();

// Identifies who the user is
app.UseAuthentication();

// Determines what the user can access
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Checks "Who are you?" (Cookies)
app.UseAuthorization();  // Checks "Allowed to be here?" (Roles)


// Default route: Landing page first, then users can navigate to Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Landing}/{action=Index}/{id?}");

app.Run();