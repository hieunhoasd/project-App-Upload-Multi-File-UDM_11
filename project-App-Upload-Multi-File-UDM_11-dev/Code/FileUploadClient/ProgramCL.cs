using System;
using System.Net.Sockets;

class Program
{
    static void Main()
    {
        // Tạo client TCP
        TcpClient client = new TcpClient();

        // Kết nối đến server trên cổng 9000
        client.Connect("127.0.0.1", 9000);

        Console.WriteLine("Connected to server!");

        // Đóng kết nối với server
        client.Close();
    }
}