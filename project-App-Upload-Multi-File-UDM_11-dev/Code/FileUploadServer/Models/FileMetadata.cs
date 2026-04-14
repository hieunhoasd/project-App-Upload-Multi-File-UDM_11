namespace FileUploadServer.Models
{
    // ĐỊNH NGHĨA METADATA CỦA FILE (Để Server nhận biết)
    public class FileMetadata
    {
        public string? FileName { get; set; }
        public long FileSize { get; set; }
    }
}