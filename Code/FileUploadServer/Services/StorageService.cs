using System;
using System.IO;
using FileUploadServer.Models;
namespace FileUploadServer.Services
{
    public class StorageService
    {
        public void SaveFile(Stream stream, FileMetadata metadata)
        {
            // Tạo thư mục lưu trữ nếu chưa tồn tại
            string uploadFolder = "Uploads";
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            string fileNameOnly = Path.GetFileNameWithoutExtension(metadata.FileName);
            string extension = Path.GetExtension(metadata.FileName);
            string filePath = Path.Combine("Uploads", metadata.FileName);
            int count = 1;

            // Vòng lặp kiểm tra trùng tên
            while (File.Exists(filePath))
            {
                // Vòng lặp kiểm tra nếu trùng thì tạo tên mới và đánh số: TenFile (1).txt...
                string newFileName = $"{fileNameOnly} ({count++}){extension}";
                filePath = Path.Combine("Uploads", newFileName);
            }

            // Đưa dữ liệu file vào thư mục đã tạo
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = new byte[65536];
                long totalRead = 0;
                int bytesRead;

                while (totalRead < metadata.FileSize && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, bytesRead);
                    totalRead += bytesRead;
                }
            }
            Console.WriteLine($"[Storage] Đã lưu xong file tại: {filePath}");
        }
    }
}