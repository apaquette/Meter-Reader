using EnergyInsightHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace EnergyInsightHub.Data;

public class EnergyHubContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public EnergyHubContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("EnergyHubDB"));
        

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Meter>()
            .HasOne(m => m.Controller)
            .WithOne(m => m.CurrentMeter)
            .HasForeignKey<Microcontroller>(m => m.Id)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Microcontroller>()
            .HasOne(m => m.CurrentMeter)
            .WithOne(m => m.Controller)
            .HasForeignKey<Meter>(m => m.Id)
            .OnDelete(DeleteBehavior.SetNull);
    }
    public DbSet<Meter> Meters { get; set; }
    public DbSet<Microcontroller> MicroControllers { get; set; }
}
