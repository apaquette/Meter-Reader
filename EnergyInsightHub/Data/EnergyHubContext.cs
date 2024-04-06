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
        modelBuilder.Entity<Reading>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd(); // Id is generated by the database
    }
    public DbSet<Reading> Readings { get; set; }
}
