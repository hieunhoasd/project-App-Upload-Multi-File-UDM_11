namespace GUI;

using System;
static class Program
{

    [STAThread]
    static void Main()
    {
        // Bắt lỗi văng ra từ luồng giao diện chính 
        Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Global_UI_ExceptionHandler);

        // Bắt lỗi văng ra từ các luồng ngầm 
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Global_Background_ExceptionHandler);

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);


        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());

        static void Global_UI_ExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Hiện thông báo thay vì crash app
            MessageBox.Show($"Đã xảy ra lỗi hệ thống: {e.Exception.Message}",
                            "Lỗi Giao Diện",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }

        static void Global_Background_ExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Lấy đối tượng lỗi ra
            Exception ex = (Exception)e.ExceptionObject;

            MessageBox.Show($"Đã xảy ra lỗi chạy ngầm: {ex.Message}",
                            "Lỗi Hệ Thống Ngầm",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }

    }
}