using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            string jsonPath = "Output.json";

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

            // Ghi ra file Json
            ReadCsv.WriteToJsonFile(jsonPath, data);

            MessageBox.Show("✅ Đọc CSV và ghi Json thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAVL_Click(object sender, EventArgs e)
        {
            string csvPath = "data.csv";
            string jsonPath = "AVL_Output.json";

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("⚠️ Không tìm thấy file data.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string[]> data = ReadCsv.ReadCsvFile(csvPath);
            if (data.Count < 2)
            {
                MessageBox.Show("⚠️ File CSV không hợp lệ hoặc không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] header = data[0];
            AVLTree tree = new AVLTree();

            // 🔹 Xác định cột Total
            int totalIndex = Array.FindIndex(header, h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));
            if (totalIndex == -1)
            {
                MessageBox.Show("⚠️ Không tìm thấy cột 'Total' trong file CSV!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 🔹 Thêm từng dòng vào cây AVL
            for (int i = 1; i < data.Count; i++)
            {
                string[] row = data[i];
                if (row.Length <= totalIndex) continue;

                if (!int.TryParse(row[totalIndex], out int total))
                    total = 0;

               
                string key = $"{total:D6}_{i:D5}";
                tree.Insert(key, row);
            }

            
            tree.SaveToJson(jsonPath, header);

            List<string[]> sorted = new List<string[]>();
            tree.InOrder(tree.Root, sorted);

            DataTable dt = new DataTable();
            foreach (string col in header)
                dt.Columns.Add(col);
            foreach (var r in sorted)
                dt.Rows.Add(r);

            dgv1.DataSource = dt;

            MessageBox.Show("✅ Đã sắp xếp theo Total và lưu ra AVL_Output.json!", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // Chuẩn hóa hàm tạo khóa sắp xếp
        private string CreateSortableKey(string datePart, string timePart)
        {
            if (int.TryParse(datePart, out int day))
            {
                // Dùng năm & tháng giả định 
                DateTime dt;
                if (DateTime.TryParseExact($"{day} {timePart}", "d h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ||
                    DateTime.TryParseExact($"{day} {timePart}", "d h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    return dt.ToString("yyyyMMddHHmmss");
                }

                // Nếu không parse được, vẫn tạo khóa an toàn
                return $"{day:00}_{timePart}";
            }

            // Nếu là chuỗi ngày chuẩn (dd/MM/yyyy hoặc yyyy-MM-dd)
            string[] formats = new[]
            {
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy H:mm:ss",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd H:mm:ss",
                "MM/dd/yyyy HH:mm:ss"
            };

            if (DateTime.TryParseExact($"{datePart} {timePart}", formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parsed))
            {
                return parsed.ToString("yyyyMMddHHmmss");
            }

            return $"{datePart}_{timePart}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string select = comboBox1.SelectedItem.ToString();
            string csvPath = "data.csv";
            string jsonPath = "AVL_Output.json";

            if (!File.Exists(csvPath))
            {
                MessageBox.Show("⚠️ Không tìm thấy file data.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string[]> data = ReadCsv.ReadCsvFile(csvPath);
            if (data.Count < 2)
            {
                MessageBox.Show("⚠️ File CSV không hợp lệ hoặc không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //  Khai báo các biến chỉ 1 lần
            string[] header = data[0];
            AVLTree tree = new AVLTree();
            int totalIndex = -1;

            //  Xác định cột cần sắp xếp
            if (select == "Total")
                totalIndex = Array.FindIndex(header, h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));
            else if (select == "TruckCount")
                totalIndex = Array.FindIndex(header, h => h.Trim().Equals("TruckCount", StringComparison.OrdinalIgnoreCase));
            else if (select == "BusCount")
                totalIndex = Array.FindIndex(header, h => h.Trim().Equals("BusCount", StringComparison.OrdinalIgnoreCase));
            else if (select == "BikeCount")
                totalIndex = Array.FindIndex(header, h => h.Trim().Equals("BikeCount", StringComparison.OrdinalIgnoreCase));
            else if (select == "CarCount")
                totalIndex = Array.FindIndex(header, h => h.Trim().Equals("CarCount", StringComparison.OrdinalIgnoreCase));

            //  sắp xếp theo ngày giờ
            if (totalIndex == -1)
            {
                // Dùng ngày + giờ làm khóa
                for (int i = 1; i < data.Count; i++)
                {
                    string[] row = data[i];
                    if (row.Length < 2) continue;

                    string timePart = row[0].Trim();
                    string datePart = row[1].Trim();

                    string key = CreateSortableKey(datePart, timePart);
                    tree.Insert(key, row);
                }
            }
            else
            {
                // Sắp theo cột được chọn
                for (int i = 1; i < data.Count; i++)
                {
                    string[] row = data[i];
                    if (row.Length <= totalIndex) continue;

                    if (!int.TryParse(row[totalIndex], out int val))
                        val = 0;

                    string key = $"{val:D6}_{i:D5}";
                    tree.Insert(key, row);
                }
            }

            //  Lưu ra file TXT
            tree.SaveToJson(jsonPath, header);

            //  Hiển thị dữ liệu đã sắp xếp
            List<string[]> sorted = new List<string[]>();
            tree.InOrder(tree.Root, sorted);

            DataTable dt = new DataTable();
            foreach (string col in header)
                dt.Columns.Add(col);
            foreach (var r in sorted)
                dt.Rows.Add(r);

            dgv1.DataSource = dt;

            MessageBox.Show($"✅ Đã sắp xếp theo {select} và lưu ra AVL_Output.json!", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}

