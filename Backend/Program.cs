using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Hubs;
using Backend.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Enable Razor runtime compilation for hot reload

builder.Services.AddRazorPages();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
