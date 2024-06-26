﻿using EnergyInsightHub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace EnergyInsightHub.Data;

/// <summary>
/// Represents the database context for the EnergyInsightHub application.
/// </summary>
public class EnergyHubContext : DbContext
{
    /// <summary>
    /// The configuration instance used for database connection settings.
    /// </summary>
    protected readonly IConfiguration Configuration;
    /// <summary>
    /// Initializes a new instance of the <see cref="EnergyHubContext"/> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration instance.</param>
    public EnergyHubContext(IConfiguration configuration)
    {
        Configuration = configuration;
        Database.EnsureCreated();
    }
    /// <summary>
    /// Configures the database connection using the provided options.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Configuration.GetConnectionString("EnergyHubDB"));
    }

    /// <summary>
    /// Configures the entity models and relationships for the database.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reading>()
                .Property(m => m.Id)
                .ValueGeneratedOnAdd(); // Id is generated by the database
    }

    /// <summary>
    /// Resets the database by removing existing readings and adding test readings from a CSV file.
    /// </summary>
    public void ResetDB() {
        Readings.RemoveRange(Readings); // delete old readings
        var testData = ReadCsv("Data\\testData.csv");
        Readings.AddRange(testData); // add test readings
        SaveChangesAsync();
    }

    /// <summary>
    /// Reads data from a CSV file and returns a list of readings.
    /// </summary>
    /// <param name="filePath">The path to the CSV file.</param>
    /// <returns>A list of <see cref="Reading"/> objects parsed from the CSV file.</returns>
    private List<Reading> ReadCsv(string filePath) {
        List<Reading> testReadings = new();

        using (TextFieldParser parser = new TextFieldParser(filePath)) {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            parser.ReadFields(); // skip first row (headers)

            while (!parser.EndOfData) {
                string[] fields = parser.ReadFields();

                // Assuming CSV structure matches object properties
                Reading reading = new(){
                    EnergyMeterId = int.Parse(fields[0]),
                    MicrocontrollerId = int.Parse(fields[1]),
                    Time = DateTime.Parse(fields[2]),
                    Amount = decimal.Parse(fields[3])
                };

                testReadings.Add(reading);
            }
        }

        return testReadings;
    }

    /// <summary>
    /// Represents a database table for storing readings.
    /// </summary>
    public DbSet<Reading> Readings { get; set; }
}
