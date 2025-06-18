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

    public DbSet<Server> Servers { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
}

