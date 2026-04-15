namespace GUI;
public partial class Form1 : Form
{
    // Tạo danh sách quản lý dữ liệu file
    public List<FileModel> FileList = new List<FileModel>();

    public Form1()
    {
        InitializeComponent();
        InitUI();
    }
    // Cập nhật hàm AddFile để vừa hiện lên UI, vừa lưu vào danh sách dữ liệu
    void AddFile(string file)
    {
        var info = new FileInfo(file);

        // Khởi tạo Model dữ liệu
        var newFile = new FileModel
        {
            FileName = info.Name,
            FileSize = info.Length,
            Status = "Waiting",
            Progress = 0,
            BytesSent = 0
        };
        FileList.Add(newFile); 
        // Thêm vào danh sách tổng

        // Đổ dữ liệu lên UI (Binding vào ListView)
        var item = new ListViewItem(new string[]
        {
            newFile.FileName,
            (newFile.FileSize / 1024).ToString() + " KB",
            "0%",
            newFile.Status
        });

        item.Tag = newFile;
        listView.Items.Add(item);
    }

    // Cập nhật hàm DragDrop để dùng chung logic AddFile
    private void ListView_DragDrop(object sender, DragEventArgs e)
    {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files != null)
        {
            foreach (string file in files)
            {
                AddFile(file); 
                // Gọi hàm để vừa lưu vào List, vừa hiện UI
            }
        }
    }
}
