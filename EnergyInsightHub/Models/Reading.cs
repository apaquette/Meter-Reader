using System.ComponentModel.DataAnnotations;

namespace EnergyInsightHub.Models;

/// <summary>
/// Represents a reading captured by an energy meter.
/// </summary>
public class Reading
{
    /// <summary>
    /// Gets or sets the unique identifier for the reading.
    /// </summary>
    [Key]
    public int Id { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the energy meter associated with the reading.
    /// </summary>
    public int EnergyMeterId { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the microcontroller that captured the reading.
    /// </summary>
    public int MicrocontrollerId { get; set; }
    /// <summary>
    /// Gets or sets the timestamp when the reading was captured.
    /// </summary>
    public DateTime Time { get; set; } //time the reading was captured
    /// <summary>
    /// Gets or sets the amount represented by the reading.
    /// </summary>
    public decimal Amount { get; set; } //amount the reading represents
    /// <summary>
    /// Initializes a new instance of the <see cref="Reading"/> class.
    /// </summary>
    public Reading() { }
}
