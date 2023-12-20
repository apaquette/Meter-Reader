namespace MeterReader.Models;
public class Reading
{
    public Meter EnergyMeter { get; private set; }
    public DateTime Time { get; private set; }
    public decimal Amount { get; private set; }
}
