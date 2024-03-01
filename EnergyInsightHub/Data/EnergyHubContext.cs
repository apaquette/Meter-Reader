﻿using EnergyInsightHub.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyInsightHub.Data;

public class EnergyHubContext : DbContext
{
    protected readonly IConfiguration Configuration;
    public EnergyHubContext(IConfiguration configuration)
    {
        Configuration = configuration;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("EnergyHubDB"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Meter>()
        //    .HasOne(m => m.Controller)
        //    .WithOne(m => m.CurrentMeter)
        //    .HasForeignKey<Microcontroller>(m => m.Id)
        //    .IsRequired(false);

        //modelBuilder.Entity<Microcontroller>()
        //    .HasOne(m => m.CurrentMeter)
        //    .WithOne(m => m.Controller)
        //    .HasForeignKey<Meter>(m => m.Id)
        //    .IsRequired(false);

        //modelBuilder.Entity<Meter>()
        //        .Property(m => m.Id)
        //        .ValueGeneratedOnAdd(); // Id is generated by the database
        //modelBuilder.Entity<Microcontroller>()
        //        .Property(m => m.Id)
        //        .ValueGeneratedOnAdd(); // Id is generated by the database
        modelBuilder.Entity<Reading>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd(); // Id is generated by the database
        //modelBuilder.Entity<MicrocontrollerAssignment>()
        //        .Property(m => m.TransactionId)
        //        .ValueGeneratedOnAdd(); // Id is generated by the database
    }
    public DbSet<Reading> Readings { get; set; }
    //public DbSet<Meter> Meters { get; set; }
    //public DbSet<Microcontroller> MicroControllers { get; set; }
}
