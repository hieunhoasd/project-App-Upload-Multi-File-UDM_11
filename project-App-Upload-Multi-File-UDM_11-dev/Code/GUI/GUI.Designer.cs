namespace GUI;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.listView = new System.Windows.Forms.ListView();
        this.colFileName = new System.Windows.Forms.ColumnHeader();
        this.colSize = new System.Windows.Forms.ColumnHeader();
        this.colProgress = new System.Windows.Forms.ColumnHeader();
        this.colSpeed = new System.Windows.Forms.ColumnHeader();
        this.colStatus = new System.Windows.Forms.ColumnHeader();
        this.btnSelect = new System.Windows.Forms.Button();
        this.btnUpload = new System.Windows.Forms.Button();
        this.lblTitle = new System.Windows.Forms.Label();

        this.SuspendLayout();

        // listView
        this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colSize,
            this.colProgress,
            this.colSpeed,
            this.colStatus});

        this.listView.FullRowSelect = true;
        this.listView.GridLines = true;
        this.listView.Location = new System.Drawing.Point(12, 50);
        this.listView.Name = "listView";
        this.listView.Size = new System.Drawing.Size(780, 380);
        this.listView.TabIndex = 0;
        this.listView.UseCompatibleStateImageBehavior = false;
        this.listView.View = System.Windows.Forms.View.Details;
        this.listView.AllowDrop = true;
        this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView_DragEnter);
        this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);

        // Columns
        this.colFileName.Text = "Tên File";
        this.colFileName.Width = 260;

        this.colSize.Text = "Kích thước";
        this.colSize.Width = 90;

        this.colProgress.Text = "Tiến trình";
        this.colProgress.Width = 110;

        this.colSpeed.Text = "Tốc độ";
        this.colSpeed.Width = 100;

        this.colStatus.Text = "Trạng thái";
        this.colStatus.Width = 140;

        // btnSelect
        this.btnSelect.Location = new System.Drawing.Point(12, 12);
        this.btnSelect.Name = "btnSelect";
        this.btnSelect.Size = new System.Drawing.Size(110, 32);
        this.btnSelect.Text = "Chọn File";
        this.btnSelect.Click += new System.EventHandler(this.BtnSelect_Click);

        // btnUpload
        this.btnUpload.Location = new System.Drawing.Point(680, 12);
        this.btnUpload.Name = "btnUpload";
        this.btnUpload.Size = new System.Drawing.Size(110, 32);
        this.btnUpload.Text = "Upload";
        this.btnUpload.Click += new System.EventHandler(this.BtnUpload_Click);

        // lblTitle
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.lblTitle.Location = new System.Drawing.Point(140, 18);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Text = "Kéo thả file vào bên dưới hoặc nhấn nút Chọn File";

        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(815, 460);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.btnUpload);
        this.Controls.Add(this.btnSelect);
        this.Controls.Add(this.listView);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "File Upload Client - Multi File Upload";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.ColumnHeader colFileName;
    private System.Windows.Forms.ColumnHeader colSize;
    private System.Windows.Forms.ColumnHeader colProgress;
    private System.Windows.Forms.ColumnHeader colSpeed;
    private System.Windows.Forms.ColumnHeader colStatus;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnUpload;
    private System.Windows.Forms.Label lblTitle;
}