namespace MeterReader.Models;

public class Meter
{
    private Microcontroller microcontroller;
    public Meter(Microcontroller controller, string serialNumber, string address)
    {
        microcontroller = controller;
        SerialNumber = serialNumber;
        Address = address;
    }

    public string SerialNumber { get; }

    public string Address { get; set; }




    //TODO: Implement GetReadings
    public List<Reading> GetReadings()
    {
        return new();
    }

    //TODO: Implement Assignment method
    public MicrocontrollerAssignment AssignMicrocontroller(Microcontroller controller)
    {
        microcontroller = controller;
        return new();
    }
}
