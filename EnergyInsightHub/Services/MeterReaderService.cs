namespace EnergyInsightHub.Services;

using System;
using System.Diagnostics;
using System.Management;

public class MeterReaderService {
    private Process _meterReader;
    private int? _processId;

    public MeterReaderService() {
        _processId = GetPid();
        StopMeterReader();
        StartMeterReader();
    }

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
    
    public void StopMeterReader() {
        if (_processId.HasValue) {
            int pid = (int)_processId;
            EndService(pid);
        }
    }

    //solution from here: https://stackoverflow.com/questions/30249873/process-kill-doesnt-seem-to-kill-the-process
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
