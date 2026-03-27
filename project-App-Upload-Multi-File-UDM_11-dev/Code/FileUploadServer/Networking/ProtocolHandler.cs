using System.IO;
using System.Net.Sockets;
using System.Text;
using FileUploadServer.Models;

namespace FileUploadServer.Networking
{
    public static class ProtocolDecoder
    {
        // Chỉ chứa logic giải mã (nhận)
        public static FileMetadata DecodeMetadata(NetworkStream stream)
        {
            // Dùng BinaryReader để đọc luồng byte từ mạng
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

            // Đọc đúng thứ tự chuẩn hóa: Tên file (String) -> Kích thước (Int64)
            string fileName = reader.ReadString();
            long fileSize = reader.ReadInt64();

            return new FileMetadata
            {
                FileName = fileName,
                FileSize = fileSize
            };
        }
    }
}