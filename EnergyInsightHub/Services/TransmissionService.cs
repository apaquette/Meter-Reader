namespace EnergyInsightHub.Services; 
public class TransmissionService {
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
