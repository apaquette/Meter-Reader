using System.Diagnostics;
using System.Management;

namespace EnergyInsightHub.Services;

/// <summary>
/// Service for managing the meter reader process.
/// </summary>
public class MeterReaderService {
    /// <summary>
    /// The meter reader process.
    /// </summary>
    private Process _meterReader;
    /// <summary>
    /// The ID of the meter reader process.
    /// </summary>
    private int? _processId;
    /// <summary>
    /// Initializes a new instance of the <see cref="MeterReaderService"/> class.
    /// </summary>
    public MeterReaderService() {
        _processId = GetPid();
        StopMeterReader();
        StartMeterReader();
    }

    /// <summary>
    /// Starts the meter reader process.
    /// </summary>
    public void StartMeterReader() 
    {
        string directoryPath = "MeterReader"; // Path to meter reader script
        string scriptName = "MeterReader.py";       // Meter Reader script
        string command = $"py {scriptName}";        // command to execute

        // Create a process start info object
        ProcessStartInfo startInfo = new() {
            FileName = "cmd.exe",
            WorkingDirectory = directoryPath,
            Arguments = $"/C {command}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        _meterReader = Process.Start(startInfo);
        File.WriteAllTextAsync(Path.Combine("pid.txt"), _meterReader.Id.ToString());
    }

    /// <summary>
    /// Stops the meter reader process.
    /// </summary>
    public void StopMeterReader() {
        if (_processId.HasValue) {
            int pid = (int)_processId;
            EndService(pid);
        }
    }


    /// <summary>
    /// Recursively ends a process and its child processes.
    /// </summary>
    /// <param name="pid">The process ID to end.</param>
    /// <remarks>Credit for solution: https://stackoverflow.com/questions/30249873/process-kill-doesnt-seem-to-kill-the-process</remarks>
    private void EndService(int pid) {
        ManagementObjectSearcher processSearcher = new ($"Select * From Win32_Process Where ParentProcessID={pid}");
        ManagementObjectCollection processCollection = processSearcher.Get();

        try {
            Process proc = Process.GetProcessById(pid);
            if (!proc.HasExited) proc.Kill();
        }
        catch (ArgumentException) {
            // Process already exited.
        }

        if (processCollection != null) {
            foreach (ManagementObject mo in processCollection) {
                EndService(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
            }
        }

    }

    /// <summary>
    /// Gets the process ID from the pid.txt file.
    /// </summary>
    /// <returns>The process ID if found, otherwise null.</returns>
    private int? GetPid() 
    {
        if (File.Exists("pid.txt")) {
            string pidString = File.ReadAllText("pid.txt");
            if (int.TryParse(pidString, out int pid)) {
                return pid;
            }
        }
        return null;
    }
}
