namespace EnergyInsightHub.Models;

//TODO: need to check if the microcontroller is already assigned, if it is, mark the previous one as the endDate
public class MicrocontrollerAssignment
{
    public Microcontroller Controller { get; set; }
    public Meter TheMeter { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
