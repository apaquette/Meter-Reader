using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyInsightHub.Models;

//TODO: need to check if the microcontroller is already assigned, if it is, mark the previous one as the endDate
public class MicrocontrollerAssignment
{
    [Key]
    public int TransactionId { get; set; }
    public Microcontroller Controller { get; set; }
    public Meter EnergyMeter { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public MicrocontrollerAssignment() { }

    public MicrocontrollerAssignment(Microcontroller microcontroller, Meter meter, DateTime startTime)
    {
        Controller = microcontroller;
        EnergyMeter = meter;
        StartTime = startTime;
    }
}
