namespace MeterReader.Models;

public class Meter
{
    private Microcontroller microcontroller;

    public string SerialNumber { get; }

    public string Address { get; set; }

    public Meter(Microcontroller controller, string serialNumber)
    {
        microcontroller = controller;
        SerialNumber = serialNumber;
    }



    //TODO: Implement GetReadings
    public List<Reading> GetReadings()
    {
        return new();
    }

    public void AssignMicrocontroller(Microcontroller controller)
    {
        microcontroller = controller;
    }
}
