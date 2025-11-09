using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using HuynhLamHuy_DH52300664;
using Microsoft.VisualBasic;


namespace HuynhLamHuy_DH52300664
{


    public partial class MainForm : Form
    {
        private List<string[]> topNRows = new List<string[]>();
        private AVLTree currentTree;
        private string[] currentHeader;
        private int currentSortColumnIndex = -1;
        public MainForm()
        {
            InitializeComponent();
        }
        // Hàm đọc CSV
        private List<string[]> ReadCsvFile(string path)
        {
            var list = new List<string[]>();
            foreach (var line in File.ReadAllLines(path))
                list.Add(line.Split(','));
            return list;
        }

        // Tạo key theo Total
        private string CreateKeyTotal(string totalStr, int rowIndex)
        {
            if (!int.TryParse(totalStr, out int val))
                val = 0;
            return $"{val:D6}_{rowIndex:D5}";
        }

        // InOrder AVL lưu AVLNode
        private void InOrderWithLevel(AVLNode node, int level, List<(AVLNode node, int level)> list)
        {
            if (node == null) return;
            InOrderWithLevel(node.Left, level + 1, list);
            list.Add((node, level));
            InOrderWithLevel(node.Right, level + 1, list);
        }
        private void FindValueWithDateTime(AVLNode node, int columnIndex, string target, List<string> results)
        {
            if (node == null) return;

            string cell = node.Data[columnIndex].Trim();

            // So sánh giá trị nhập với ô dữ liệu
            bool match = false;
            if (double.TryParse(cell, out double val1) && double.TryParse(target, out double val2))
                match = Math.Abs(val1 - val2) < 0.0001;
            else
                match = cell.Equals(target, StringComparison.OrdinalIgnoreCase);

            if (match)
            {
                // Giả sử: cột ngày = 0, cột giờ = 1
                string time = node.Data[0];
                string date = node.Data[1];
                string value = node.Data[columnIndex];

                results.Add($" Ngày: {date}   Giờ: {time}   Giá trị: {value}");
            }

            // Duyệt cây trái & phải
            FindValueWithDateTime(node.Left, columnIndex, target, results);
            FindValueWithDateTime(node.Right, columnIndex, target, results);
        }


        private string ShowInputDialog(string text, string title)
        {
            return Microsoft.VisualBasic.Interaction.InputBox(text, title, "");
        }
        private int CountValueOccurrences(AVLNode node, int columnIndex, string targetValue)
        {
            if (node == null) return 0;
            int count = 0;

            if (columnIndex >= 0 && columnIndex < node.Data.Length)
            {
                string cellValue = node.Data[columnIndex].Trim();

                // So sánh số: "18" == "18.0"
                if (double.TryParse(cellValue, out double cellNum) &&
                    double.TryParse(targetValue, out double targetNum))
                {
                    if (Math.Abs(cellNum - targetNum) < 0.0001)
                        count++;
                }
                // Hoặc so sánh chuỗi thường
                else if (cellValue.Equals(targetValue, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                }
            }

            count += CountValueOccurrences(node.Left, columnIndex, targetValue);
            count += CountValueOccurrences(node.Right, columnIndex, targetValue);

            return count;
        }



        private void btnGhiKetQua_Click(object sender, EventArgs e)
        {
            string csvPath = "data.csv";
            string jsonPath = "Output.json";

            if (!File.Exists(csvPath))
            {
                MessageBox.Show(" Không tìm thấy file TrafficData.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            MessageBox.Show(" Đọc CSV và ghi Json thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(" Không tìm thấy file data.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string[]> data = ReadCsv.ReadCsvFile(csvPath);
            if (data.Count < 2)
            {
                MessageBox.Show(" File CSV không hợp lệ hoặc không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            currentTree = tree;
            currentHeader = header;
            currentSortColumnIndex = totalIndex;
            //  Lưu ra file json
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

            MessageBox.Show($" Đã sắp xếp theo {select} và lưu ra AVL_Output.json!", "Thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentTree == null || currentHeader == null)
            {
                MessageBox.Show(" Hãy chọn cột sắp xếp (ComboBox1) trước!", "Chưa có cây AVL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string select = comboBox2.SelectedItem.ToString();
            AVLTree tree = currentTree;

            if (select == "Chiều cao cây")
            {
                MessageBox.Show($" Chiều cao cây: {tree.GetHeight()}", "Kết quả",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (select == "Đếm node có 2 node lá")
            {
                int count = tree.CountNodesWithTwoLeafChildren(tree.Root);
                MessageBox.Show($" Số node có 2 lá : {count}", "Kết quả",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (select == "Giá trị nhỏ nhất")
            {
                var min = tree.FindMin(tree.Root);
                if (min != null)
                {
                    // Lấy giá trị thật trong dòng dữ liệu, dựa vào currentSortColumnIndex
                    string realValue = (currentSortColumnIndex >= 0 && currentSortColumnIndex < currentHeader.Length)
                        ? min.Data[currentSortColumnIndex]
                        : min.Key;

                    MessageBox.Show($" Giá trị nhỏ nhất theo {currentHeader[currentSortColumnIndex]}: {realValue}",
                        "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            else if (select == "Giá trị lớn nhất")
            {
                var max = tree.FindMax(tree.Root);
                if (max != null)
                {
                    string realValue = (currentSortColumnIndex >= 0 && currentSortColumnIndex < currentHeader.Length)
                        ? max.Data[currentSortColumnIndex]
                        : max.Key;

                    MessageBox.Show($" Giá trị lớn nhất theo {currentHeader[currentSortColumnIndex]}: {realValue}",
                        "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            else if (select == "Tìm giá trị")
            {
                if (currentSortColumnIndex < 0)
                {
                    MessageBox.Show("⚠ Không thể tìm kiếm khi đang sắp theo ngày/giờ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string input = ShowInputDialog(
                    $"Nhập giá trị cần tìm theo {currentHeader[currentSortColumnIndex]}:",
                    "Tìm kiếm");

                if (string.IsNullOrWhiteSpace(input)) return;

                List<string> results = new List<string>();
                FindValueWithDateTime(currentTree.Root, currentSortColumnIndex, input, results);

                if (results.Count > 0)
                {
                    string msg = $" Tìm thấy {results.Count} kết quả:\n\n" + string.Join("\n", results);
                    MessageBox.Show(msg, "Kết quả tìm kiếm", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($" Không tìm thấy giá trị '{input}' trong cột {currentHeader[currentSortColumnIndex]}!",
                        "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (select == "đếm lá")
            {
                int count = tree.CountLeafNodes(tree.Root);
                MessageBox.Show($" Số node lá trong cây: {count}", "Kết quả",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        private void btnTopN_Click(object sender, EventArgs e)
        {
            string csvPath = "data.csv";
            if (!File.Exists(csvPath))
            {
                MessageBox.Show(" Không tìm thấy file data.csv!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = ReadCsvFile(csvPath);
            if (data.Count < 2)
            {
                MessageBox.Show(" CSV không hợp lệ hoặc không có dữ liệu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            currentHeader = data[0]; // lưu header

            string input = Interaction.InputBox("Nhập số dòng muốn hiển thị:", "Top N", "10");
            if (!int.TryParse(input, out int n) || n <= 0)
            {
                MessageBox.Show(" Số dòng không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            n = Math.Min(n, data.Count - 1); // trừ header
            topNRows.Clear();
            for (int i = 1; i <= n; i++)
                topNRows.Add(data[i]);

            // Hiển thị lên DataGridView
            DataTable dt = new DataTable();
            foreach (var col in currentHeader)
                dt.Columns.Add(col);

            foreach (var row in topNRows)
                dt.Rows.Add(row);

            dgv1.DataSource = dt;
            MessageBox.Show($" Đã hiển thị {topNRows.Count} dòng đầu tiên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnDuyet_Click(object sender, EventArgs e)
        {
            if (topNRows.Count == 0)
            {
                MessageBox.Show(" Hãy xuất Top N trước!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tìm index cột Total
            int totalIndex = Array.FindIndex(currentHeader, h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));
            if (totalIndex < 0)
            {
                MessageBox.Show(" Không tìm thấy cột Total!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Tạo cây AVL từ topNRows theo Total
            currentTree = new AVLTree();
            for (int i = 0; i < topNRows.Count; i++)
            {
                string key = CreateKeyTotal(topNRows[i][totalIndex], i);
                currentTree.Insert(key, topNRows[i]);
            }

            string input = Interaction.InputBox("Nhập tầng muốn duyệt (root=0):", "Duyệt tầng K", "0");
            if (!int.TryParse(input, out int level) || level < 0)
            {
                MessageBox.Show(" Tầng không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy node theo tầng
            List<(AVLNode node, int level)> nodesWithLevel = new List<(AVLNode, int)>();
            InOrderWithLevel(currentTree.Root, 0, nodesWithLevel);

            List<string[]> result = new List<string[]>();
            foreach (var (node, nodeLevel) in nodesWithLevel)
            {
                if (nodeLevel == level)
                    result.Add(node.Data);
            }

            if (result.Count == 0)
            {
                MessageBox.Show($" Không có node nào ở tầng {level}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Hiển thị kết quả
            DataTable dt = new DataTable();
            foreach (var col in currentHeader)
                dt.Columns.Add(col);

            foreach (var row in result)
                dt.Rows.Add(row);

            dgv1.DataSource = dt;
            MessageBox.Show($" Hiển thị {result.Count} node ở tầng {level}!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


}


