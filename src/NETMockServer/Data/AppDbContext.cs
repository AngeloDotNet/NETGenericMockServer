using Microsoft.EntityFrameworkCore;
using NETMockServer.Entities;

namespace NETMockServer.Data;

public class AppDbContext(DbContextOptions<AppDbContext> opts) : DbContext(opts)
{

    // Optional explicit DbSets (not strictly necessary if you use Set<T>())
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure entity types are part of the model. Add new entities here.
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Customer>();
    }
}