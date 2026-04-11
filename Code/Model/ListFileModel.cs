namespace GUI;

public class FileModel
{
    public string FileName { get; set; } = "";
    public long FileSize { get; set; }
    public long BytesSent { get; set; }
    public double Progress { get; set; }
    public string Status { get; set; } = "Waiting";
    public double Speed { get; set; }
}
