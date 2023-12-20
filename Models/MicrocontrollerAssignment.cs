namespace MeterReader.Models;

//TODO: need to check if the microcontroller is already assigned, if it is, mark the previous one as the endDate
public class MicrocontrollerAssignment
{
    Microcontroller controller;
    Meter meter;
    DateTime startTime;
    DateTime? endTime;

    public MicrocontrollerAssignment(Microcontroller controller, Meter meter, DateTime startTime, DateTime? endTime)
    {
        this.controller = controller;
        this.meter = meter;
        this.startTime = startTime;
        this.endTime = endTime;
    }
}
