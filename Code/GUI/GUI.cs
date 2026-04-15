using System.Drawing;
using System.Net.Quic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using FileUploadClient.Networking;

namespace GUI;

public partial class Form1 : Form
{
    private ListView listView;
    private Button btnSelect;
    private Button btnUpload;
    private ClientConnection _connection;

    public Form1()
    {
        InitializeComponent();
        InitUI();
        _connection = new ClientConnection("127.0.0.1", 9000);
    }

    private void InitUI()
    {

        listView = new ListView();
        listView.View = View.Details;
        listView.Columns.Add("File Name", 200);
        listView.Columns.Add("Size", 100);
        listView.Columns.Add("Progress", 100);
        listView.Columns.Add("Status", 100);
        listView.Bounds = new Rectangle(10, 10, 500, 300);

        listView.AllowDrop = true;
        listView.DragEnter += ListView_DragEnter;
        listView.DragDrop += ListView_DragDrop;

        btnSelect = new Button();
        btnSelect.Text = "Select";
        btnSelect.Bounds = new Rectangle(520, 10, 80, 30);
        btnSelect.Click += BtnSelect_Click;

        btnUpload = new Button();
        btnUpload.Text = "Upload";
        btnUpload.Bounds = new Rectangle(250, 320, 100, 40);
        btnUpload.Click += BtnUpload_Click;

        this.Controls.Add(listView);
        this.Controls.Add(btnSelect);
        this.Controls.Add(btnUpload);
    }

    private void ListView_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
        else
            e.Effect = DragDropEffects.None;
    }

    private void ListView_DragDrop(object sender, DragEventArgs e)
    {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        foreach (string file in files)
        {
            var item = new ListViewItem(new string[] { Path.GetFileName(file), new FileInfo(file).Length.ToString(), "0%", "Pending" });
            item.Tag = file;
            listView.Items.Add(item);
        }
    }

    void AddFile(string file)
    {
        //validation file 0kb (rỗng không có gì trong file)
        FileInfo fi = new FileInfo(file);
        if (fi.Length == 0)
        {
            MessageBox.Show($"File {fi.Name} trống rỗng, không thể upload!", "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; // Dừng luôn, không add vào ListView nữa
        }

        //validation trùng file đã có trong danh sách upload
        foreach (ListViewItem existingItem in listView.Items)
        {
            if (existingItem.Tag != null && existingItem.Tag.ToString() == file)
            {
                MessageBox.Show($"File {fi.Name} đã có trong danh sách rồi!", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        try
        {
            var item = new ListViewItem(new string[]
                   {
            Path.GetFileName(file),
            (new FileInfo(file).Length / 1024) + "KB",
            "0%",
            "Waiting"
                   });
            item.Tag = file;
            listView.Items.Add(item);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể thêm file {Path.GetFileName(file)}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }

    private void BtnSelect_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog ofd = new OpenFileDialog())
        {
            ofd.Multiselect = true;
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
        if (!_connection.IsConnected)
        {
            await _connection.ConnectAsync();
        }

        //validation chua chon file da nhan upload
        if (listView.Items.Count == 0)
        {
            MessageBox.Show("Vui lòng chọn hoặc kéo thả ít nhất 1 file để upload nhé!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var controller = new UploadController(_connection, listView);
        await controller.StartUploadAllAsync();
    }
}


