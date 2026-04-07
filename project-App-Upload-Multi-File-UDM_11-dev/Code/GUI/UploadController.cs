using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileUploadClient.Models;
using FileUploadClient.Networking;

namespace GUI
{
    public class UploadController
    {
        private readonly ClientConnection _connection;
        private readonly ListView _listView;

        private const int ChunkSize = 4096; // 4KB mỗi chunk (theo giai đoạn 2)

        public UploadController(ClientConnection connection, ListView listView)
        {
            _connection = connection;
            _listView = listView;
        }

        // ── Upload toàn bộ file trong ListView ──
        public async Task StartUploadAllAsync()
        {    // Lấy Stream 1 lần duy nhất cho toàn bộ phiên upload
            NetworkStream stream = _connection.GetStream();
            // Gưi đếm 1 lần duy nhất
            var pendingItems = _listView.Items
            .Cast<ListViewItem>()
            .Where(i => i.SubItems[3].Text != "Done")
            .ToList();

            if (pendingItems.Count == 0) return;
            using var writer = new BinaryWriter(stream,
            System.Text.Encoding.UTF8, leaveOpen: true);
            writer.Write(pendingItems.Count);
            writer.Flush();
            // Truyền stream xuống và không tạo lại
            foreach (var item in pendingItems)
                await UploadOneFileAsync(item, stream);

        }

        // ── Upload 1 file ──
        private async Task UploadOneFileAsync(ListViewItem item)
        {
            string filePath = item.Tag as string ?? "";

            if (!File.Exists(filePath))
            {
                UpdateStatus(item, "Error");
                MessageBox.Show($"Không tìm thấy file: {filePath}");
                return;
            }

            try
            {
                UpdateStatus(item, "Uploading");

                NetworkStream stream = _connection.GetStream();

                var metadata = new FileMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    FileSize = new FileInfo(filePath).Length
                };

                ProtocolEncoder.EncodeMetadata(stream, metadata);
                await SendFileChunksAsync(stream, item, filePath, metadata.FileSize);

                UpdateStatus(item, "Done");
                UpdateProgress(item, 100);
            }
            catch (Exception ex)
            {
                UpdateStatus(item, "Error");
                MessageBox.Show($"Lỗi upload: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Gửi chunk + cập nhật Progress ──
        private async Task SendFileChunksAsync(NetworkStream stream, ListViewItem item,
            string filePath, long fileSize)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[ChunkSize];
            long totalSent = 0;
            int bytesRead;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long lastBytes = 0;

            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);
                totalSent += bytesRead;

                double progress = fileSize > 0 ? (double)totalSent / fileSize * 100 : 0;
                UpdateProgress(item, progress);

                if (stopwatch.ElapsedMilliseconds >= 500)
                {
                    long bytesDelta = totalSent - lastBytes;
                    double speed = bytesDelta / (stopwatch.ElapsedMilliseconds / 1000.0);
                    UpdateSpeed(item, speed);
                    lastBytes = totalSent;
                    stopwatch.Restart();
                }
            }
        }

        private void UpdateProgress(ListViewItem item, double progress)
        {
            if (_listView.InvokeRequired)
                _listView.Invoke(() => item.SubItems[2].Text = $"{progress:F1}%");
            else
                item.SubItems[2].Text = $"{progress:F1}%";
        }

        private void UpdateStatus(ListViewItem item, string status)
        {
            if (_listView.InvokeRequired)
                _listView.Invoke(() => item.SubItems[3].Text = status);
            else
                item.SubItems[3].Text = status;
        }

        private void UpdateSpeed(ListViewItem item, double speed)
        {
            string speedText = speed < 1024 * 1024
                ? $"{speed / 1024:F1} KB/s"
                : $"{speed / (1024 * 1024):F1} MB/s";
            Console.WriteLine($"[{item.SubItems[0].Text}] Speed: {speedText}");
        }
    }
}
