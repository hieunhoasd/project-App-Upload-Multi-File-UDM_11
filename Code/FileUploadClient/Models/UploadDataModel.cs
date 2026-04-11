using System;
using System.IO;

namespace FileUploadClient.Models
{
    // Tạo model UploadFile
    public class UploadFile
    {
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        public long FileSize => new FileInfo(FilePath).Length;
    }

    // Tạo model dữ liệu 
    public class UploadDataModel
    {
        private UploadFile[] files;
        private int count;

        public UploadDataModel(int size = 10)
        {
            files = new UploadFile[size];
            count = 0;
        }

        public void AddFile(UploadFile file)
        {
            if (count == files.Length) Resize();
            files[count] = file;
            count++;
        }

        private void Resize()
        {
            int newSize = files.Length * 2;
            UploadFile[] newArray = new UploadFile[newSize];
            for (int i = 0; i < files.Length; i++) newArray[i] = files[i];
            files = newArray;
        }

        public UploadFile GetFile(int index)
        {
            if (index < 0 || index >= count) throw new IndexOutOfRangeException();
            return files[index];
        }

        public int Count() => count;
    }
}