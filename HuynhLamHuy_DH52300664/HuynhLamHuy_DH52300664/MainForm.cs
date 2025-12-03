using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private AVLTree primitiveTree;               // Cây không có giá trị trùng
        private List<string[]> duplicateList = new List<string[]>();   // Danh sách lưu trùng
        private Dictionary<int, List<string[]>> duplicateMap = new Dictionary<int, List<string[]>>();

        public MainForm()
        {
            InitializeComponent();
        }
        private void XuLy_TimGiaTriTrung()
        {
            string input = Interaction.InputBox("Nhập giá trị Total cần tìm trùng:",
                                                "Tìm giá trị trùng", "");

            if (!int.TryParse(input, out int val))
            {
                MessageBox.Show("Giá trị không hợp lệ!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!duplicateMap.ContainsKey(val))
            {
                MessageBox.Show($"Không có phần tử nào trùng với {val}.",
                    "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowRowsOnGrid(duplicateMap[val]);
        }

        private void ShowRowsOnGrid(List<string[]> rows)
        {
            DataTable dt = new DataTable();
            foreach (var col in currentHeader)
                dt.Columns.Add(col);

            foreach (var row in rows)
                dt.Rows.Add(row);

            dgv1.DataSource = dt;
        }

        private void XuLy_LietKeTatCa()
        {
            DataTable dt = new DataTable();
            foreach (var col in currentHeader)
                dt.Columns.Add(col);

            foreach (var pair in duplicateMap)
            {
                foreach (var row in pair.Value)
                    dt.Rows.Add(row);
            }

            dgv1.DataSource = dt;

            MessageBox.Show("Đã liệt kê tất cả giá trị trùng!",
                "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void XuLy_TimSoTrungItNhat()
        {
            int minCount = duplicateMap.Values.Min(list => list.Count);

            var result = duplicateMap
                .Where(kvp => kvp.Value.Count == minCount)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            string msg = "Giá trị trùng ít nhất:\n";
            foreach (var item in result)
                msg += $"- Giá trị {item.Key} xuất hiện {item.Value.Count} lần\n";

            MessageBox.Show(msg, "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void XuLy_TimSoTrungNhieuNhat()
        {
            int maxCount = duplicateMap.Values.Max(list => list.Count);

            var result = duplicateMap
                .Where(kvp => kvp.Value.Count == maxCount)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            string msg = "Giá trị trùng nhiều nhất:\n";
            foreach (var item in result)
                msg += $"- Giá trị {item.Key} xuất hiện {item.Value.Count} lần\n";

            MessageBox.Show(msg, "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Hàm lấy tất cả node theo tầng (hỗ trợ nhiều tầng)
        private List<AVLNode> GetNodesAtLevel(AVLNode root, int targetLevel)
        {
            List<AVLNode> result = new List<AVLNode>();
            if (root == null) return result;

            Queue<(AVLNode node, int level)> q = new Queue<(AVLNode, int)>();
            q.Enqueue((root, 0));

            while (q.Count > 0)
            {
                var (node, level) = q.Dequeue();

                if (level == targetLevel)
                {
                    result.Add(node);
                }

                if (node.Left != null) q.Enqueue((node.Left, level + 1));
                if (node.Right != null) q.Enqueue((node.Right, level + 1));
            }

            return result;
        }

        private List<AVLNode> GetNodesByLevels(AVLNode root, IEnumerable<int> levels)
        {
            List<AVLNode> result = new List<AVLNode>();
            if (root == null || levels == null) return result;

            Queue<(AVLNode node, int level)> queue = new Queue<(AVLNode node, int level)>();
            queue.Enqueue((root, 0));

            HashSet<int> levelSet = new HashSet<int>(levels);

            while (queue.Count > 0)
            {
                var (node, level) = queue.Dequeue();
                if (levelSet.Contains(level))
                    result.Add(node);

                if (node.Left != null)
                    queue.Enqueue((node.Left, level + 1));
                if (node.Right != null)
                    queue.Enqueue((node.Right, level + 1));
            }

            return result;
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
        // Tạo cây theo nút TopN
        private AVLTree BuildAVLFromTopN()
        {
            if (topNRows.Count == 0)
                return null;

            int totalIndex = Array.FindIndex(currentHeader,
                h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));

            if (totalIndex < 0)
            {
                MessageBox.Show("Không tìm thấy cột Total!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            AVLTree tree = new AVLTree();

            for (int i = 0; i < topNRows.Count; i++)
            {
                string key = CreateKeyTotal(topNRows[i][totalIndex], i);
                tree.Insert(key, topNRows[i]);
            }
            currentSortColumnIndex = totalIndex;

            return tree;
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
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentTree == null)
            {
                MessageBox.Show("Hãy bấm Hiện N trước!", "Chưa có Top N",
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
            else if (select == "duyệt lá")
            {
                var leaves = currentTree.GetLeafNodes();
                if (leaves.Count == 0)
                {
                    MessageBox.Show("Không có node lá!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable dt = new DataTable();
                foreach (var col in currentHeader)
                    dt.Columns.Add(col);

                foreach (var leaf in leaves)
                    dt.Rows.Add(leaf.Data);

                dgv1.DataSource = dt;  // Quan trọng: gán DataSource mới
                MessageBox.Show($"Có {leaves.Count} node lá!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (select == "Duyệt lá chẵn")
            {
                if (currentTree == null)
                {
                    MessageBox.Show("Hãy bấm Hiện N trước!", "Chưa có cây",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<AVLNode> leaves = currentTree.GetLeafNodes();
                List<string[]> evenLeafRows = new List<string[]>();

                foreach (var leaf in leaves)
                {
                    if (int.TryParse(leaf.Data[currentSortColumnIndex], out int val))
                    {
                        if (val % 2 == 0)
                            evenLeafRows.Add(leaf.Data);
                    }
                }

                if (evenLeafRows.Count == 0)
                {
                    MessageBox.Show("Không có lá chẵn!", "Kết quả",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị DGV
                DataTable dt = new DataTable();
                foreach (var col in currentHeader)
                    dt.Columns.Add(col);

                foreach (var row in evenLeafRows)
                    dt.Rows.Add(row);

                dgv1.DataSource = dt;
            }
            else if (select == "Duyệt lá lẻ")
            {
                if (currentTree == null)
                {
                    MessageBox.Show("Hãy bấm Hiện N trước!", "Chưa có cây",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<AVLNode> leaves = currentTree.GetLeafNodes();
                List<string[]> oddLeafRows = new List<string[]>();

                foreach (var leaf in leaves)
                {
                    if (int.TryParse(leaf.Data[currentSortColumnIndex], out int val))
                    {
                        if (val % 2 != 0)
                            oddLeafRows.Add(leaf.Data);
                    }
                }

                if (oddLeafRows.Count == 0)
                {
                    MessageBox.Show("Không có lá lẻ!", "Kết quả",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị DGV
                DataTable dt = new DataTable();
                foreach (var col in currentHeader)
                    dt.Columns.Add(col);

                foreach (var row in oddLeafRows)
                    dt.Rows.Add(row);

                dgv1.DataSource = dt;
            }

            else if (select == "tổng các lá")
            {
                if (currentSortColumnIndex < 0)
                {
                    MessageBox.Show("Không tìm thấy cột Total!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sum = currentTree.SumOddLeafTotals(currentSortColumnIndex) + currentTree.SumEvenLeafTotals(currentSortColumnIndex);
                MessageBox.Show($"Tổng các giá trị Total ở node lá = {sum}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (select == "tổng các lá lẻ")
            {
                if (currentSortColumnIndex < 0)
                {
                    MessageBox.Show("Không tìm thấy cột Total!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sum = currentTree.SumOddLeafTotals(currentSortColumnIndex);
                MessageBox.Show($"Tổng các giá trị Total LẺ ở node lá = {sum}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (select == "tổng các lá chẵn")
            {
                if (currentSortColumnIndex < 0)
                {
                    MessageBox.Show("Không tìm thấy cột Total!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sum = currentTree.SumEvenLeafTotals(currentSortColumnIndex);
                MessageBox.Show($"Tổng các giá trị Total CHẴN ở node lá = {sum}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (select == "duyệt cây")
            {
                if (currentTree == null)
                {
                    MessageBox.Show("Cây chưa được tạo từ Top N!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hàm duyệt cây theo BFS để giữ thứ tự left -> right
                List<AVLNode> nodes = new List<AVLNode>();
                Queue<AVLNode> queue = new Queue<AVLNode>();
                if (currentTree.Root != null)
                    queue.Enqueue(currentTree.Root);

                while (queue.Count > 0)
                {
                    AVLNode node = queue.Dequeue();
                    nodes.Add(node);

                    if (node.Left != null) queue.Enqueue(node.Left);
                    if (node.Right != null) queue.Enqueue(node.Right);
                }

                if (nodes.Count == 0)
                {
                    MessageBox.Show("Cây rỗng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị dữ liệu node lên DataGridView
                DataTable dt = new DataTable();
                foreach (var col in currentHeader)
                    dt.Columns.Add(col);

                foreach (var node in nodes)
                    dt.Rows.Add(node.Data);

                dgv1.DataSource = null;
                dgv1.DataSource = dt;

                MessageBox.Show($"Đã duyệt {nodes.Count} node theo node -> left -> right!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (select == "tổng tầng")
            {
                if (currentTree == null)
                {
                    MessageBox.Show("Hãy bấm Hiện N trước!", "Chưa có cây",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string input = Interaction.InputBox("Nhập tầng muốn tính tổng (root = 0):",
                                                    "Tổng theo tầng", "0");

                if (!int.TryParse(input, out int level) || level < 0)
                {
                    MessageBox.Show("Tầng không hợp lệ!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<AVLNode> nodes = GetNodesAtLevel(currentTree.Root, level);

                if (nodes.Count == 0)
                {
                    MessageBox.Show($"Không có node nào ở tầng {level}!",
                        "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int totalIndex = Array.FindIndex(currentHeader,h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));

                if (totalIndex < 0)
                {
                    MessageBox.Show("Không tìm thấy cột Total!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sum = 0;
                foreach (var node in nodes)
                {
                    if (int.TryParse(node.Data[totalIndex], out int val))
                        sum += val;
                }

                MessageBox.Show($"Tổng Total tại tầng {level} = {sum}",
                    "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else if (select == "duyệt tầng")
            {
                if (currentTree == null)
                {
                    MessageBox.Show("Hãy bấm Hiện N trước!", "Chưa có cây",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string input = Interaction.InputBox("Nhập tầng muốn duyệt (ví dụ: 0 hoặc 1,2,3):", "Duyệt tầng K", "0");
                if (string.IsNullOrWhiteSpace(input)) return;

                string[] parts = input.Split(',');
                HashSet<int> levels = new HashSet<int>();
                foreach (var p in parts)
                    if (int.TryParse(p.Trim(), out int level) && level >= 0)
                        levels.Add(level);

                if (levels.Count == 0)
                {
                    MessageBox.Show("Tầng không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Lấy node theo tầng trả về AVLNode
                var nodes = GetNodesByLevels(currentTree.Root, levels);

                if (nodes.Count == 0)
                {
                    MessageBox.Show($"Không có node nào ở các tầng {string.Join(", ", levels)}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Hiển thị dữ liệu node nếu muốn
                DataTable dt = new DataTable();
                foreach (var col in currentHeader)
                    dt.Columns.Add(col);

                foreach (var node in nodes)
                    dt.Rows.Add(node.Data);

                dgv1.DataSource = null;
                dgv1.DataSource = dt;

                MessageBox.Show($"Hiển thị {nodes.Count} node ở các tầng: {string.Join(", ", levels)}!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            currentTree = BuildAVLFromTopN();
            MessageBox.Show($" Đã hiển thị {topNRows.Count} dòng đầu tiên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTaoCayNguyenThuy_Click(object sender, EventArgs e)
        {
            if (topNRows.Count == 0)
            {
                MessageBox.Show("Hãy bấm Hiện N trước!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int totalIndex = Array.FindIndex(currentHeader,
                h => h.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase));

            if (totalIndex < 0)
            {
                MessageBox.Show("Không tìm thấy cột Total!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            primitiveTree = new AVLTree();
            duplicateList.Clear();
            duplicateMap.Clear();   // ⚠️ Quan trọng – reset map trùng

            HashSet<int> seen = new HashSet<int>();

            for (int i = 0; i < topNRows.Count; i++)
            {
                if (!int.TryParse(topNRows[i][totalIndex], out int val))
                    continue;

                if (!seen.Contains(val))
                {
                    // Giá trị mới → đưa vào cây
                    seen.Add(val);
                    primitiveTree.Insert(val.ToString(), topNRows[i]);
                }
                else
                {
                    // Giá trị trùng → đưa vào danh sách liên kết
                    duplicateList.Add(topNRows[i]);

                    // Đồng thời lưu vào duplicateMap
                    if (!duplicateMap.ContainsKey(val))
                        duplicateMap[val] = new List<string[]>();

                    duplicateMap[val].Add(topNRows[i]);
                }
            }

            MessageBox.Show(
                $"Tạo cây nguyên thủy thành công!\n" +
                $"- Số phần tử duy nhất: {seen.Count}\n" +
                $"- Số phần tử trùng: {duplicateList.Count}",
                "Thành công",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            List<string[]> inorderList = new List<string[]>();
            primitiveTree.InOrder(primitiveTree.Root, inorderList);

            DataTable dt = new DataTable();
            foreach (var col in currentHeader)
                dt.Columns.Add(col);

            foreach (var row in inorderList)
                dt.Rows.Add(row);

            dgv1.DataSource = dt;

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (duplicateMap.Count == 0)
            {
                MessageBox.Show("không có giá trị trùng trong cây.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string select = comboBox3.SelectedItem.ToString();

            if (select == "Tìm giá trị trùng")
            {
                XuLy_TimGiaTriTrung();
            }
            else if (select == "Tìm số trùng nhiều nhất")
            {
                XuLy_TimSoTrungNhieuNhat();
            }
            else if (select == "Tìm số trùng ít nhất")
            {
                XuLy_TimSoTrungItNhat();
            }
            else if (select == "Liệt kê tất cả giá trị trùng")
            {
                XuLy_LietKeTatCa();
            }
        }
    }


}


