namespace MeterReader.Models;

public class Microcontroller
{
    private Meter? currentMeter;
    public Microcontroller(Meter? currentMeter, int id)
    {
        this.currentMeter = currentMeter;
        Id = id;
    }

    public int Id { get; private set; }
}
