using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using FileUploadServer.Services; // Gọi để dùng StorageService(logic luu file)
using FileUploadServer.Models;
using FileUploadServer.Networking;  //Gọi dùng cấu hình file mà sever sẽ nhận 
namespace FileUploadServer
{
    class ProgramSV
    {
        static void Main(string[] args)

        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            // ─── 1. TẠO SERVER ───
            // Tạo server TCP gắn với tất cả IP của máy, dùng port 9000
            TcpListener server = new TcpListener(IPAddress.Any, 9000);

            // ─── 2. LISTEN PORT ───
            // Bắt đầu lắng nghe kết nối
            server.Start();
            Console.WriteLine("Server started...");
            Console.WriteLine("Listening on port 9000...");

            // ─── 3. ACCEPT CLIENT TRUY CẬP ───
            // Vòng lặp vô hạn để liên tục chờ và chấp nhận kết nối từ nhiều client
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for client...");

                    // Lệnh AcceptTcpClient() sẽ chặn luồng và chờ đến khi có client truy cập
                    TcpClient client = server.AcceptTcpClient();

                    // Client đã kết nối thành công
                    Console.WriteLine($"Client connected: {client.Client.RemoteEndPoint}");

                    // Xử lý client ở một thread riêng (tránh block server khi có nhiều client)
                    Task.Run(() => HandleClient(client));
                }

                // Bắt lỗi khi gặp lỗi server
                catch (Exception ex)
                {
                    Console.WriteLine("Server error: " + ex.Message);
                }
            }
        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                // Thiết lập timeout cho client
                client.ReceiveTimeout = 30000;
                client.SendTimeout = 30000;

                // Xử lý dữ liệu từ client
                Console.WriteLine("Handling client...");
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Client is connected and ready.");

                // (Phần 3 & 4: Protocol và Storage sẽ được nhúng vào vị trí này sau)



                StorageService storageService = new StorageService();
                using BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

                // 1. Nhận số lượng file từ Client
                int fileCount = reader.ReadInt32();
                Console.WriteLine($"Client chuẩn bị gửi {fileCount} file(s).");

                // 2. Vòng lặp nhận từng file
                for (int i = 0; i < fileCount; i++)
                {
                    // Giải mã lấy Metadata
                    FileMetadata currentFile = ProtocolDecoder.DecodeMetadata(stream);
                    Console.WriteLine($"[{i + 1}/{fileCount}] Đang nhận: {currentFile.FileName} ({currentFile.FileSize} bytes)");

                    // Lưu file vào ổ cứng
                    storageService.SaveFile(stream, currentFile);
                }
                Console.WriteLine("Đã nhận và lưu toàn bộ file thành công!");



                // Đóng kết nối với client 
                client.Close();

                Console.WriteLine("Client disconnected.");
            }

            // Bắt lỗi khi gặp lỗi client
            catch (Exception ex)
            {
                Console.WriteLine("Client error: " + ex.Message);
            }
        }
    }
}