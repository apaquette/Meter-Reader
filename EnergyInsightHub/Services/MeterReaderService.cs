namespace EnergyInsightHub.Services;
using System.Diagnostics;

public class MeterReaderService {
    private Process _meterReader;
    private string _processName = "MeterReader";

    public MeterReaderService() {
        RestartService(_processName);
        StartMeterReader();
    }

    public void StartMeterReader() 
    {
        // only start script if it's not already running
        if(!IsProcessRunning(_processName)) {
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
        }
    }

    public void StopMeterReader() 
    {
        if(_meterReader != null && !_meterReader.HasExited ) {
            _meterReader.Kill();
            _meterReader?.Dispose();
        }
    }

    // check if the process is running
    private bool IsProcessRunning(string processName) {
        Process[] processes = Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }

    private void RestartService(string processName) {
        Process[] processes = Process.GetProcessesByName(processName);
        foreach( Process process in processes ) {
            process.Kill();
            process?.Dispose();
        }
    }
}
