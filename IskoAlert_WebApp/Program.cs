using IskoAlert_WebApp.Data;
using IskoAlert_WebApp.Models.Domain;
using IskoAlert_WebApp.Models.Domain.Enums;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

   
    context.Database.EnsureCreated();

    // Testing
    if (!context.Users.Any())
    {
        context.Users.Add(
            new User(
                idNumber: "2021-00001-MN-0",
                webmail: "admin@iskomail.pup.edu.ph",
                passwordHash: "TestPassword123",
                name: "PUP Admin",
                role: UserRole.Admin
    )
);
        context.SaveChanges();
        Console.WriteLine("Seed Data: Test User created successfully!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// FIX: We use the standard 'UseStaticFiles' to ensure images/css load correctly
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

// FIX: This sets the startup page to Account/Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
