using System.ComponentModel.DataAnnotations;

namespace EnergyInsightHub.Models;
public class Reading
{
    [Key]
    public int Id { get; set; }
    public Meter EnergyMeter { get; set; } //Energy Meter the reading came from
    public Microcontroller Microcontroller { get; set; } //Microcontroller that generated the reading
    public DateTime Time { get; set; } //time the reading was captured
    public decimal Amount { get; set; } //amount the reading represents

    public Reading() { }
}
