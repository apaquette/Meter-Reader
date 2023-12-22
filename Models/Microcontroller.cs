using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyInsightHub.Models;

public class Microcontroller
{
    [Key]
    public int Id { get; set; }
    public Meter? CurrentMeter { get; set; }
    public List<Reading> Readings { get; set; } = new();
    public List<MicrocontrollerAssignment> Assignments { get; set; } = new();


    public List<Reading> GetReadings(Meter meter)
    {
        return new();
    }
    //TODO: Implement
    public void AssignToMeter(Meter meter)
    {
        //check if previous assignment with null EndDate
            //if yes, update EndDate

        //Update CurrentMeter
        //Create new assignment with new meter and set startDate
    }
}
