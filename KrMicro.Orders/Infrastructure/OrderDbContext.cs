using KrMicro.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Orders.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext()
    {
    }

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<PaymentMethod> Payments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<DeliveryInformation> DeliveryInformations { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>().HasOne<PaymentMethod>(t => t.PaymentMethod).WithMany(p => p.Transactions)
            .HasForeignKey("PaymentMethodId");

        modelBuilder.Entity<Transaction>().Navigation(t => t.PaymentMethod).AutoInclude();

        modelBuilder.Entity<Order>().HasMany<OrderDetail>(o => o.OrderDetails).WithOne(d => d.Order)
            .HasForeignKey("OrderId").IsRequired();

        modelBuilder.Entity<Order>().Navigation(o => o.OrderDetails).AutoInclude();
        modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderId, od.ProductId, od.SizeId });


        modelBuilder.Entity<Order>().HasOne<DeliveryInformation>(o => o.DeliveryInformation).WithMany(d => d.Orders)
            .HasForeignKey("DeliveryInformationId").IsRequired();

        modelBuilder.Entity<Order>().Navigation(o => o.DeliveryInformation).AutoInclude();

        modelBuilder.Entity<Transaction>().HasOne<Order>(t => t.Order).WithMany(o => o.Transactions)
            .HasForeignKey("OrderId_Transaction").IsRequired();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        options.UseNpgsql(configuration.GetConnectionString("DbSQL"));
    }
}