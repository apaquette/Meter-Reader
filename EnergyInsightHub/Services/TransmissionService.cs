namespace EnergyInsightHub.Services;
/// <summary>
/// Service for transmitting images from one directory to another.
/// </summary>
/// <remarks>This service simulates the hardware component of the solution.</remarks>
public class TransmissionService {
    /// <summary>
    /// Transmits images from one directory to another asynchronously with a 2.5 second delay between transmissions.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task TransmitImages() {
        await Task.Run(() =>
        {
            string sourceDir = "TestMeterImages";
            string destinationDir = "MeterReader";
            List<string> imageFiles = Directory.GetFiles(sourceDir, "*.png").ToList();
            foreach (string image in imageFiles) {
                string newImageDir = Path.Combine(destinationDir, Path.GetFileName(image));
                File.Move(image, newImageDir);
                Thread.Sleep(2500);
            }
        });
    }
}
