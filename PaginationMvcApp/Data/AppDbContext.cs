using Microsoft.EntityFrameworkCore;
using PaginationMvcApp.Models;

namespace PaginationMvcApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed 30 products
        var products = new List<Product>();
        for (int i = 1; i <= 30; i++)
        {
            products.Add(new Product
            {
                Id = i,
                Name = $"Product {i}",
                Price = 10.00m + (i * 5)
            });
        }

        modelBuilder.Entity<Product>().HasData(products);
    }
}
