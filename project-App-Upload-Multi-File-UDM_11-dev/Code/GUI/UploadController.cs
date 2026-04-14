using System;
using System.Diagnostics;
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

        private const int ChunkSize = 65536;           // 64KB - tốt cho tốc độ
        private const int MinUpdateIntervalMs = 120;   // Chỉ update UI mỗi 120ms 

        public UploadController(ClientConnection connection, ListView listView)
        {
            _connection = connection;
            _listView = listView;
        }

        public async Task StartUploadAllAsync()
        {
            NetworkStream stream = _connection.GetStream();

            var pendingItems = _listView.Items
                .Cast<ListViewItem>()
                .Where(i => i.SubItems[3].Text != "Done" && i.SubItems[3].Text != "Error")
                .ToList();

            if (pendingItems.Count == 0) return;

            // Gửi số lượng file
            using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
            writer.Write(pendingItems.Count);
            writer.Flush();

            foreach (var item in pendingItems)
            {
                await UploadOneFileAsync(item, stream);
            }
        }

        private async Task UploadOneFileAsync(ListViewItem item, NetworkStream stream)
        {
            string filePath = item.Tag?.ToString() ?? "";
            if (!File.Exists(filePath))
            {
                UpdateStatus(item, "Error");
                return;
            }

            try
            {
                UpdateStatus(item, "Uploading");

                var metadata = new FileMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    FileSize = new FileInfo(filePath).Length
                };

                ProtocolEncoder.EncodeMetadata(stream, metadata);

                await SendFileWithProgressAsync(stream, item, filePath, metadata.FileSize);

                UpdateStatus(item, "Done");
                UpdateProgress(item, 100);
            }
            catch (Exception ex)
            {
                UpdateStatus(item, "Error");
                MessageBox.Show($"Lỗi upload {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private async Task SendFileWithProgressAsync(NetworkStream stream, ListViewItem item,
            string filePath, long fileSize)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, ChunkSize, true);

            byte[] buffer = new byte[ChunkSize];
            long totalSent = 0;
            DateTime lastUpdateTime = DateTime.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            long lastBytesSent = 0;

            int bytesRead;
            while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);
                totalSent += bytesRead;

                // === THROTTLING + SMOOTH UI ===
                if ((DateTime.UtcNow - lastUpdateTime).TotalMilliseconds >= MinUpdateIntervalMs)
                {
                    double progress = fileSize > 0 ? (double)totalSent / fileSize * 100 : 0;
                    progress = Math.Min(100.0, Math.Max(0.0, progress)); // Không vượt 100%

                    UpdateProgress(item, progress);

                    // Tính tốc độ
                    double seconds = stopwatch.Elapsed.TotalSeconds;
                    if (seconds > 0.8)
                    {
                        double speedKBps = (totalSent - lastBytesSent) / 1024.0 / seconds;
                        UpdateSpeed(item, speedKBps);
                        lastBytesSent = totalSent;
                        stopwatch.Restart();
                    }

                    lastUpdateTime = DateTime.UtcNow;
                }
            }

            // Update lần cuối
            UpdateProgress(item, 100);
        }

        private void UpdateProgress(ListViewItem item, double progress)
        {
            string text = $"{progress:F1}%";
            if (_listView.InvokeRequired)
                _listView.Invoke(() => item.SubItems[2].Text = text);
            else
                item.SubItems[2].Text = text;
        }

        private void UpdateStatus(ListViewItem item, string status)
        {
            if (_listView.InvokeRequired)
                _listView.Invoke(() => item.SubItems[3].Text = status);
            else
                item.SubItems[3].Text = status;
        }

        private void UpdateSpeed(ListViewItem item, double speedKBps)
        {
            string speedText = speedKBps >= 1024
                ? $"{(speedKBps / 1024):F1} MB/s"
                : $"{speedKBps:F1} KB/s";

            if (_listView.InvokeRequired)
                _listView.Invoke(() => item.SubItems[3].Text = speedText);   // Cột Speed là SubItems[3]
            else
                item.SubItems[3].Text = speedText;
        }
    }
}   