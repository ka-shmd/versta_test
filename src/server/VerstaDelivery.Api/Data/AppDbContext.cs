using Microsoft.EntityFrameworkCore;
using VerstaDelivery.Api.Models;

namespace VerstaDelivery.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(order =>
        {
            order.HasIndex(o => o.OrderNumber).IsUnique();

            order.Property(o => o.OrderNumber).HasMaxLength(20);

            order.Property(o => o.SenderCity).HasMaxLength(100);
            order.Property(o => o.SenderAddress).HasMaxLength(255);
            order.Property(o => o.RecipientCity).HasMaxLength(100);
            order.Property(o => o.RecipientAddress).HasMaxLength(255);

            order.Property(o => o.Weight).HasPrecision(10, 2);
        });
    }
}
