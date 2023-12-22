namespace MeterReader.Models;
public class Reading
{
    public Meter EnergyMeter { get; set; }
    public DateTime Time { get; set; }
    public decimal Amount { get; set; }
}
