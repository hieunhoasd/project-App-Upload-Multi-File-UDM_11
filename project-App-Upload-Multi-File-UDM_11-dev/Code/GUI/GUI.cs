using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FileUploadClient.Networking;

namespace GUI;

public partial class Form1 : Form
{
    private ClientConnection _connection;

    public Form1()
    {
        InitializeComponent();
        _connection = new ClientConnection("127.0.0.1", 9000);
    }

    // ====================== EVENT HANDLERS ======================

    private void ListView_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
    }

    private void ListView_DragDrop(object sender, DragEventArgs e)
    {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        foreach (string file in files)
        {
            AddFile(file);
        }
    }

    private void BtnSelect_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Multiselect = true;
            ofd.Title = "Chọn file để upload";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                {
                    AddFile(file);
                }
            }
        }
    }

    private async void BtnUpload_Click(object sender, EventArgs e)
    {
        if (listView.Items.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn ít nhất 1 file!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            if (!_connection.IsConnected)
                await _connection.ConnectAsync();

            var controller = new UploadController(_connection, listView);
            await controller.StartUploadAllAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ====================== THÊM FILE ======================
    private void AddFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        FileInfo fi = new FileInfo(filePath);

        if (fi.Length == 0)
        {
            MessageBox.Show($"File {fi.Name} trống, không thể upload!", "Cảnh báo",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Kiểm tra trùng lặp
        foreach (ListViewItem item in listView.Items)
        {
            if (item.Tag?.ToString() == filePath)
            {
                MessageBox.Show($"File {fi.Name} đã tồn tại trong danh sách!", "Trùng lặp");
                return;
            }
        }

        var lvi = new ListViewItem(new string[]
        {
        fi.Name,
        (fi.Length / 1024.0).ToString("F1") + " KB",
        "0%",
        "0 KB/s",      // Speed
        "Waiting"
        });

        lvi.Tag = filePath;
        listView.Items.Add(lvi);
    }
}