using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileUploadServer
{
    class ProgramSV
    {
        static void Main(string[] args)
        {
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