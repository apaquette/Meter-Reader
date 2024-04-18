using System.ComponentModel.DataAnnotations;

namespace EnergyInsightHub.Models;
public class Reading
{
    [Key]
    public int Id { get; set; }
    public int EnergyMeterId { get; set; }
    public int MicrocontrollerId { get; set; }
    public DateTime Time { get; set; } //time the reading was captured
    public decimal Amount { get; set; } //amount the reading represents

    public Reading() { }
}
