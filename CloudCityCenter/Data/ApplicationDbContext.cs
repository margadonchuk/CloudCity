using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CloudCityCenter.Models;

namespace CloudCityCenter.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
    public DbSet<ProductFeature> ProductFeatures { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<Server> Servers { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
    public DbSet<BlockedIp> BlockedIps { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        builder.Entity<Order>()
            .Property(o => o.Status)
            .HasDefaultValue(OrderStatus.Pending);

        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId);

        builder.Entity<Product>()
            .Property(p => p.PricePerMonth)
            .HasPrecision(18, 2);

        builder.Entity<Product>()
            .Property(p => p.IsPublished)
            .HasDefaultValue(false);

        builder.Entity<ProductVariant>()
            .Property(v => v.Price)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(i => i.Price)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.Total)
            .HasPrecision(18, 2);

        builder.Entity<Product>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        builder.Entity<Server>()
            .HasIndex(s => s.Slug)
            .IsUnique();

        builder.Entity<Server>()
            .Property(s => s.PricePerMonth)
            .HasPrecision(18, 2);

        builder.Entity<Server>()
            .Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Entity<Server>()
            .Property(s => s.DDoSTier)
            .HasDefaultValue("Basic");

        builder.Entity<Server>()
            .Property(s => s.Stock)
            .HasDefaultValue(9999);


        builder.Entity<BlockedIp>(entity =>
        {
            entity.ToTable("BlockedIps");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.IpAddress)
                .IsRequired()
                .HasMaxLength(45);

            entity.Property(x => x.Reason)
                .HasMaxLength(500);

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);

            entity.HasIndex(x => new { x.IpAddress, x.IsActive })
                .IsUnique();

            if (Database.IsSqlServer())
            {
                entity.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            }
            else
            {
                entity.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        });

        if (Database.IsSqlServer())
        {
            builder.Entity<Server>()
                .Property(s => s.CreatedUtc)
                .HasDefaultValueSql("GETUTCDATE()");
        }
        else
        {
            builder.Entity<Server>()
                .Property(s => s.CreatedUtc)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
