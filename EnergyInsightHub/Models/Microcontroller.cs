using System.ComponentModel.DataAnnotations;

namespace EnergyInsightHub.Models;

public class Microcontroller
{
    [Key]
    public int Id { get; private set; }
    public Meter? CurrentMeter { get; private set; }
    public List<Reading> Readings { get; private set; } = new();
    public List<MicrocontrollerAssignment> Assignments { get; private set; } = new();

    public Microcontroller() { } // Parameterless constructor for Entity Framework

    //public List<Reading> GetReadings(Meter meter) => Readings.Where(r => r.EnergyMeter == meter).ToList();

    //TODO: Implement
    public void AssignToMeter(Meter meter)
    {
        var lastAssignment = Assignments.Where(t => t.EndTime != null && t.EnergyMeter.Id == meter.Id && t.Controller.Id == Id).ToList().FirstOrDefault();
        DateTime assignTime = DateTime.Now;
        if (lastAssignment != null)
        {
            lastAssignment.EndTime = assignTime;
        }

        //Update CurrentMeter
        CurrentMeter = meter;
        CurrentMeter.Controller = this;
        Assignments.Add(new(this, CurrentMeter, assignTime));
    }
}
