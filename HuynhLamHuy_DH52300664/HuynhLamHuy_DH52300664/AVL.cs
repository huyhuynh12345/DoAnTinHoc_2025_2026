using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HuynhLamHuy_DH52300664
{
    public class AVLNode
    {
        public string Key;
        public string[] Data;
        public int Height;
        public AVLNode Left, Right;

        public AVLNode(string key, string[] data)
        {
            Key = key;
            Data = data;
            Height = 1;
        }
    }

    public class AVLTree
    {
        public AVLNode Root;

        private int Height(AVLNode n) => n?.Height ?? 0;

        private int GetBalance(AVLNode n)
        {
            if (n == null) return 0;
            return Height(n.Left) - Height(n.Right);
        }

        private AVLNode RightRotate(AVLNode y)
        {
            AVLNode x = y.Left;
            AVLNode T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

            return x;
        }

        private AVLNode LeftRotate(AVLNode x)
        {
            AVLNode y = x.Right;
            AVLNode T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
            y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

            return y;
        }

        public AVLNode Insert(AVLNode node, string key, string[] data)
        {
            if (node == null)
                return new AVLNode(key, data);

            int cmp = string.Compare(key, node.Key, StringComparison.Ordinal);
            if (cmp < 0)
                node.Left = Insert(node.Left, key, data);
            else if (cmp > 0)
                node.Right = Insert(node.Right, key, data);
            else
            {
                // Nếu trùng khóa thì cập nhật dữ liệu
                node.Data = data;
                return node;
            }

            node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
            int balance = GetBalance(node);

            // Xử lý mất cân bằng
            if (balance > 1 && string.Compare(key, node.Left.Key, StringComparison.Ordinal) < 0)
                return RightRotate(node);

            if (balance > 1 && string.Compare(key, node.Left.Key, StringComparison.Ordinal) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && string.Compare(key, node.Right.Key, StringComparison.Ordinal) > 0)
                return LeftRotate(node);

            if (balance < -1 && string.Compare(key, node.Right.Key, StringComparison.Ordinal) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public void Insert(string key, string[] data)
        {
            Root = Insert(Root, key, data);
        }

        public void InOrder(AVLNode node, List<string[]> rows)
        {
            if (node == null) return;
            InOrder(node.Left, rows);
            rows.Add(node.Data);
            InOrder(node.Right, rows);
        }

        public void SaveToJson(string path, string[] header)
        {
            List<string[]> rows = new List<string[]>();
            InOrder(Root, rows);

            var list = new List<Dictionary<string, string>>();
            foreach (var r in rows)
            {
                var obj = new Dictionary<string, string>();
                for (int i = 0; i < header.Length && i < r.Length; i++)
                    obj[header[i]] = r[i];
                list.Add(obj);
            }

            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
