using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] values = line.Split(',');
                    rows.Add(values);
                }

                Console.WriteLine("✅ Đọc CSV thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi đọc CSV: {ex.Message}");
            }

            return rows;
        }

        // 🔹 Ghi file JSON bằng Newtonsoft.Json
        public static void WriteToJsonFile(string filePath, List<string[]> data)
        {
            try
            {
                if (data.Count == 0)
                {
                    Console.WriteLine("⚠️ Không có dữ liệu để ghi JSON!");
                    return;
                }

                // Dòng đầu là tiêu đề
                string[] headers = data[0];
                var list = new List<Dictionary<string, string>>();

                // Các dòng sau là dữ liệu
                for (int i = 1; i < data.Count; i++)
                {
                    var obj = new Dictionary<string, string>();
                    string[] row = data[i];

                    for (int j = 0; j < headers.Length && j < row.Length; j++)
                    {
                        obj[headers[j]] = row[j];
                    }

                    list.Add(obj);
                }

                // Ghi JSON format đẹp
                string json = JsonConvert.SerializeObject(list, Formatting.Indented);
                File.WriteAllText(filePath, json);

                Console.WriteLine($"✅ Ghi dữ liệu JSON thành công: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Lỗi khi ghi JSON: {ex.Message}");
            }
        }
    }
}
