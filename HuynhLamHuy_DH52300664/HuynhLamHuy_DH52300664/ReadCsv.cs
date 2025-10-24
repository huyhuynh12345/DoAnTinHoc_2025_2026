using System;
using System.Collections.Generic;
using System.IO;

namespace HuynhLamHuy_DH52300664
{
    public static class ReadCsv
    {
        // 🔹 Hàm đọc CSV
        public static List<string[]> ReadCsvFile(string filePath)
        {
            var rows = new List<string[]>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Bỏ qua dòng trống
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] values = line.Split(',');
                    rows.Add(values);
                }

                Console.WriteLine("Đọc CSV thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc CSV: {ex.Message}");
            }

            return rows;
        }

        // 🔹 Hàm ghi TXT
        public static void WriteToTxtFile(string filePath, List<string[]> data)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (string[] row in data)
                    {
                        writer.WriteLine(string.Join(" | ", row));
                    }
                }

                Console.WriteLine($"✅ Ghi dữ liệu vào file TXT thành công: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi ghi file TXT: {ex.Message}");
            }
        }
    }
}
