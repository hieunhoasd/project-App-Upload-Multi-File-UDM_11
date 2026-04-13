using System;
using System.IO;
using System.Net.Sockets;
using FileUploadClient.Models;
using FileUploadClient.Networking;
using System.Threading.Tasks; // Cần thêm cái này để dùng async/await

namespace FileUploadClient
{
    class ProgramCL
    {
        // Phải đổi thành async Task Main vì ClientConnection của bạn dùng async
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            // 1. GIÁM ĐỐC GỌI THỦ KHO (Lên danh sách hàng)
            UploadDataModel uploadList = new UploadDataModel();

            // 2. GIÁM ĐỐC GỌI TÀI XẾ (Khởi động xe, kết nối mạng)
            ClientConnection connection = new ClientConnection("127.0.0.1", 9000);

            try
            {
                await connection.ConnectAsync(); // Ra lệnh tài xế chạy tới Server
                NetworkStream stream = connection.GetStream(); // Lấy đường truyền

                // Gửi số lượng file trước
                using BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
                writer.Write(uploadList.Count());
                writer.Flush();

                // 3. GIÁM ĐỐC GỌI NHÂN VIÊN ĐÓNG GÓI (Vòng lặp gửi từng file)
                for (int i = 0; i < uploadList.Count(); i++)
                {
                    UploadFile fileToUpload = uploadList.GetFile(i);

                    FileMetadata metadata = new FileMetadata
                    {
                        FileName = fileToUpload.FileName,
                        FileSize = fileToUpload.FileSize
                    };

                    // Gọi Encoder mã hóa gửi đi
                    ProtocolEncoder.EncodeMetadata(stream, metadata);

                    // Bơm dữ liệu file vào mạng
                    using (FileStream fs = new FileStream(fileToUpload.FilePath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] buffer = new byte[65536];
                        int bytesRead;
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                    Console.WriteLine($"Đã gửi xong: {metadata.FileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
            finally
            {
                // Xong việc thì ra lệnh tài xế tắt máy xe
                connection.Disconnect();
            }

            Console.ReadLine();
        }
    }
}