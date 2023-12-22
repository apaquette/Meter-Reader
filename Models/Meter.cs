using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyInsightHub.Models;

public class Meter
{
    [Key]
    public int Id { get; set; }
    public string SerialNumber { get; set; }
    public Microcontroller? Controller { get; set; }
    public string Address { get; set; }    
}
