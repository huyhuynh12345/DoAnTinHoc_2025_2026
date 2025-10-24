using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using HuynhLamHuy_DH52300664;

namespace HuynhLamHuy_DH52300664
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnGhiKetQua_Click(object sender, EventArgs e)
        {
            string csvPath = "data.csv";
            string txtPath = "Output.txt";

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("⚠️ Không tìm thấy file TrafficData.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string[]> data = ReadCsv.ReadCsvFile(csvPath);

            if (data.Count == 0)
            {
                MessageBox.Show("File CSV trống hoặc không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo DataTable để hiển thị
            DataTable dt = new DataTable();

            // Dòng đầu tiên là tiêu đề
            foreach (string col in data[0])
                dt.Columns.Add(col.Trim());

            // Các dòng tiếp theo là dữ liệu
            for (int i = 1; i < data.Count; i++)
                dt.Rows.Add(data[i]);

            dgv1.DataSource = dt;

            // Ghi ra file TXT
            ReadCsv.WriteToTxtFile(txtPath, data);

            MessageBox.Show("✅ Đọc CSV và ghi TXT thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

