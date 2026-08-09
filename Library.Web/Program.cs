using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Library.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. Register Services
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IChallanRepository, ChallanRepository>();
builder.Services.AddScoped<ILibraryTransactionRepository, LibraryTransactionRepository>();
builder.Services.AddHttpClient<RecommendationApiService>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8000/");
});
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpClient(
    "LibraryChatbot",
    client =>
    {
        client.BaseAddress =
            new Uri("http://127.0.0.1:8001");

        client.Timeout =
            TimeSpan.FromSeconds(30);
    });
builder.Services.AddHttpClient(
    "NoiseApi",
    client =>
    {
        client.BaseAddress =
            new Uri("http://127.0.0.1:8002");

        client.Timeout =
            TimeSpan.FromSeconds(5);
    }
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// 2. Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Required for your CSS/Styles to load
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// 3. Define Routes (Must be BEFORE app.Run)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 4. Start the Application (This must be the LAST line)
app.Run();