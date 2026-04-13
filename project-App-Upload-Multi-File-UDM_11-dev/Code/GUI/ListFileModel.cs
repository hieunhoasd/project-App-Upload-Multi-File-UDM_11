namespace GUI;
public class FileModel
{
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }                  // Kích th??c file
    public long BytesSent { get; set; } = 0;           // S? byte ?ã g?i ( ?? tính progress)
    public double Progress { get; set; } = 0;          // % hoàn thành (0 - 100)
    public string Status { get; set; } = "Waiting";
    public double Speed { get; set; } = 0;             // T?c ?? KB/s ho?c MB/s
    public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;  // Dùng ?? tính speed chính xác
}
