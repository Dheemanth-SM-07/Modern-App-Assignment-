using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");
                
                entity.HasIndex(e => e.Name);
            });
            
            // Seed initial data
            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "Premium Laptop", 
                    Description = "High-performance laptop with latest processor", 
                    Price = 1299.99m,
                    InStock = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Wireless Mouse", 
                    Description = "Ergonomic wireless mouse with precision tracking", 
                    Price = 45.99m,
                    InStock = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product 
                { 
                    Id = 3, 
                    Name = "Mechanical Keyboard", 
                    Description = "RGB mechanical keyboard with blue switches", 
                    Price = 129.99m,
                    InStock = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
