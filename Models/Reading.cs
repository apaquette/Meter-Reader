namespace MeterReader.Models;
public class Reading
{
    public Reading(Meter energyMeter, DateTime time, decimal amount)
    {
        EnergyMeter = energyMeter;
        Time = time;
        Amount = amount;
    }
    public Meter EnergyMeter { get; private set; }
    public DateTime Time { get; private set; }
    public decimal Amount { get; private set; }
}
