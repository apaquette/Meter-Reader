using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyInsightHub.Models;

public class Meter
{
    [Key]
    public int Id { get; private set; }
    public string SerialNumber { get; private set; }
    public Microcontroller? Controller { get; set; }
    public string Address { get; private set; }

    public Meter() { } // Parameterless constructor for Entity Framework

    public Meter(string serialNumber, string address)
    {
        SerialNumber = serialNumber;
        Address = address;
    }
}
