using System;
using System.IO;
using FileUploadServer.Models;
namespace FileUploadServer.Services
{
    public class StorageService
    {
        public void SaveFile(Stream stream, FileMetadata metadata)
        {
            string uploadFolder = "Uploads";
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string fullFolderPath = Path.GetFullPath(uploadFolder);
            Console.WriteLine($"[Storage] Thư mục Uploads: {fullFolderPath}");

            string filePath = Path.Combine(uploadFolder, metadata.FileName);
            string baseName = Path.GetFileNameWithoutExtension(metadata.FileName) ?? "Unknown";
            string extension = Path.GetExtension(metadata.FileName) ?? "";

            int count = 1;
            while (File.Exists(filePath))
            {
                string newName = $"{baseName} ({count++}){extension}";
                filePath = Path.Combine(uploadFolder, newName);
            }

            Console.WriteLine($"[Storage] Đang lưu: {Path.GetFileName(filePath)}");

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[65536];
                    long totalRead = 0;
                    int bytesRead;

                    // Đọc đúng số byte theo FileSize
                    while (totalRead < metadata.FileSize)
                    {
                        int toRead = (int)Math.Min(buffer.Length, metadata.FileSize - totalRead);
                        bytesRead = stream.Read(buffer, 0, toRead);

                        if (bytesRead == 0)
                        {
                            Console.WriteLine("[Storage] Cảnh báo: Client ngắt kết nối giữa chừng");
                            break;
                        }

                        fs.Write(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                    }
                }

                long savedSize = new FileInfo(filePath).Length;
                Console.WriteLine($"[Storage] ✓ ĐÃ LƯU XONG: {Path.GetFileName(filePath)} ({savedSize} bytes)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Storage] LỖI lưu file: {ex.Message}");
            }
        }
    }
}