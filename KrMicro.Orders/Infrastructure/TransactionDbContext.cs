using KrMicro.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace KrMicro.Orders.Infrastructure;

public class TransactionDbContext : DbContext
{
    public TransactionDbContext(){}
    
    public TransactionDbContext(DbContextOptions<TransactionDbContext> options): base(options){}
    
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Models.Transaction> Transactions{ get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>().HasOne<Payment>(t => t.Payment).WithMany(p => p.Transactions)
            .HasForeignKey("PaymentId");

        modelBuilder.Entity<Transaction>().Navigation(t => t.Payment).AutoInclude();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        options.UseSqlServer(configuration.GetConnectionString("DbSQL"));
    }
}