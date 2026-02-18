using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Hubs;
using Backend.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Enable Razor runtime compilation for hot reload

builder.Services.AddRazorPages();

// Add DbContext - Using In-Memory Database for demonstration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ModernAppDB"));
    
// Alternative: SQL Server (uncomment when SQL Server is available)
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add HealthChecks with custom GC check
builder.Services.AddHealthChecks()
    .AddGCInfoCheck("GCInfo");

// HealthChecks UI removed due to initialization issues - core HealthChecks still functional

// Add Swagger/OpenAPI using NSwag
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Modern App API - Upgraded to .NET 10";
    config.Version = "v1";
    config.Description = "Full-stack application demonstrating migration from .NET 7 to .NET 10 with latest libraries";
});

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", 
            "http://localhost:5173",
            "https://localhost:3000",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Seed the in-memory database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Ensure database is created
    context.Database.EnsureCreated();
    
    // Add sample products if none exist
    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product
            {
                Name = "Laptop",
                Description = "High-performance laptop with 16GB RAM",
                Price = 1299.99m,
                InStock = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with USB receiver",
                Price = 29.99m,
                InStock = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with blue switches",
                Price = 89.99m,
                InStock = false,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "USB-C Hub",
                Description = "7-in-1 USB-C hub with HDMI and SD card reader",
                Price = 49.99m,
                InStock = true,
                CreatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Webcam HD",
                Description = "1080p HD webcam with built-in microphone",
                Price = 79.99m,
                InStock = true,
                CreatedAt = DateTime.Now
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseResponseCompression();
}

// Enable Swagger/OpenAPI
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "Modern App API - .NET 10";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/v1/swagger.json";
});

// Map HealthChecks endpoints
app.MapHealthChecks("/health-json", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true
});

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files from wwwroot

// Enable CORS - MUST come before UseRouting
app.UseCors("AllowReactApp");

app.UseRouting();

app.UseAuthorization();

// Map SignalR Hub
app.MapHub<NotificationHub>("/hubs/notifications");

// Map Razor Pages and MVC Controllers
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API Controllers
app.MapControllers();

app.Run();
