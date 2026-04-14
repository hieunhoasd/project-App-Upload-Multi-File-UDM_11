using System.IO;
using System.Net.Sockets;
using System.Text;
using FileUploadClient.Models;
namespace FileUploadClient.Networking
{
    public static class ProtocolEncoder
    {
        // Chỉ chứa logic mã hóa (gửi)
        public static void EncodeMetadata(NetworkStream stream, FileMetadata metadata)
        {
            // Dùng BinaryWriter để ghi luồng byte lên mạng
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

            // Ghi đúng thứ tự chuẩn hóa: Tên file (String) -> Kích thước (Int64)
            writer.Write(metadata.FileName);
            writer.Write(metadata.FileSize);

            // Ép dữ liệu đi ngay lập tức
            writer.Flush();
        }
    }
}