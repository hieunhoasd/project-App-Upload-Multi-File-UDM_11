using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadClient.Networking
{
    public class ClientConnection
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool IsConnected => _tcpClient != null && _tcpClient.Connected;

        public ClientConnection(string host, int port)
        {
            Host = host;
            Port = port;
        }

        // ─── 1. CONNECT SERVER ───
        public async Task ConnectAsync()
        {
            if (IsConnected)
                throw new InvalidOperationException("Đã kết nối rồi.");

            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(Host, Port);
            _stream = _tcpClient.GetStream();

            Console.WriteLine($"[ClientConnection] Đã kết nối tới {Host}:{Port}");
        }

        // ─── 2. DISCONNECT ───
        public void Disconnect()
        {
            if (!IsConnected)
                return;

            _stream?.Close();
            _tcpClient?.Close();

            _stream = null;
            _tcpClient = null;

            Console.WriteLine("[ClientConnection] Đã ngắt kết nối.");
        }

        // ─── 3. SEND TEST MESSAGE ───
        public async Task SendTestMessageAsync(string message = "HELLO_SERVER")
        {
            if (!IsConnected || _stream == null)
                throw new InvalidOperationException("Chưa kết nối tới server.");

            // Gửi message dạng UTF-8, kết thúc bằng newline
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            await _stream.WriteAsync(data, 0, data.Length);

            Console.WriteLine($"[ClientConnection] Đã gửi: {message}");

            // Đọc phản hồi từ server (nếu có)
            byte[] buffer = new byte[1024];
            _stream.ReadTimeout = 3000; // chờ tối đa 3 giây
            try
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"[ClientConnection] Server phản hồi: {response}");
            }
            catch (IOException)
            {
                Console.WriteLine("[ClientConnection] Server không phản hồi (timeout).");
            }
        }

        // ── Lấy stream để các class khác dùng (FileSender,...) ──
        public NetworkStream GetStream()
        {
            if (_stream == null || !IsConnected)
                throw new InvalidOperationException("Chưa kết nối tới server.");
            return _stream;
        }
    }
}