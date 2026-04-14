namespace GUI;
public class FileModel
{
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }                  // Kích thước file
    public long BytesSent { get; set; } = 0;           // Số byte đã gửi (dùng để tính progress)
    public double Progress { get; set; } = 0;          // % hoàn thành (0 - 100)
    public string Status { get; set; } = "Waiting";
    public double Speed { get; set; } = 0;             // Tốc độ upload (KB/s)
    public DateTime LastUpdateTime { get; set; } = DateTime.UtcNow;  // Dùng để tính speed chính xác
}
                                         