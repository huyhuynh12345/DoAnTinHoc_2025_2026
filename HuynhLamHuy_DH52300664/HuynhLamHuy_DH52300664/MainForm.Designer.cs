using System;
using System.Windows.Forms;

namespace HuynhLamHuy_DH52300664
{
    partial class MainForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnGhiKetQua;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnGhiKetQua = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.btnTopN = new System.Windows.Forms.Button();
            this.btnDuyet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGhiKetQua
            // 
            this.btnGhiKetQua.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGhiKetQua.Location = new System.Drawing.Point(22, 16);
            this.btnGhiKetQua.Name = "btnGhiKetQua";
            this.btnGhiKetQua.Size = new System.Drawing.Size(150, 63);
            this.btnGhiKetQua.TabIndex = 0;
            this.btnGhiKetQua.Text = "Ghi Kết Quả";
            this.btnGhiKetQua.UseVisualStyleBackColor = true;
            this.btnGhiKetQua.Click += new System.EventHandler(this.btnGhiKetQua_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Total",
            "TruckCount",
            "BusCount",
            "BikeCount",
            "CarCount",
            "DayTime"});
            this.comboBox1.Location = new System.Drawing.Point(322, 30);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(153, 37);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(208, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 28);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cây AVL :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(565, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 20);
            this.label2.TabIndex = 5;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.Font = new System.Drawing.Font("MS Reference Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Chiều cao cây",
            "Đếm node có 2 node lá",
            "Giá trị nhỏ nhất",
            "Giá trị lớn nhất",
            "Tìm giá trị",
            "đếm lá"});
            this.comboBox2.Location = new System.Drawing.Point(646, 30);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(180, 36);
            this.comboBox2.TabIndex = 6;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // dgv1
            // 
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(22, 88);
            this.dgv1.Name = "dgv1";
            this.dgv1.ReadOnly = true;
            this.dgv1.RowHeadersWidth = 62;
            this.dgv1.Size = new System.Drawing.Size(1368, 500);
            this.dgv1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(504, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "Thuật toán";
            // 
            // btnTopN
            // 
            this.btnTopN.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTopN.Location = new System.Drawing.Point(881, 30);
            this.btnTopN.Name = "btnTopN";
            this.btnTopN.Size = new System.Drawing.Size(146, 35);
            this.btnTopN.TabIndex = 9;
            this.btnTopN.Text = "Hiện N hàng";
            this.btnTopN.UseVisualStyleBackColor = true;
            this.btnTopN.Click += new System.EventHandler(this.btnTopN_Click);
            // 
            // btnDuyet
            // 
            this.btnDuyet.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDuyet.Location = new System.Drawing.Point(1057, 30);
            this.btnDuyet.Name = "btnDuyet";
            this.btnDuyet.Size = new System.Drawing.Size(144, 37);
            this.btnDuyet.TabIndex = 10;
            this.btnDuyet.Text = "Duyệt Tầng";
            this.btnDuyet.UseVisualStyleBackColor = true;
            this.btnDuyet.Click += new System.EventHandler(this.btnDuyet_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1424, 600);
            this.Controls.Add(this.btnDuyet);
            this.Controls.Add(this.btnTopN);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.btnGhiKetQua);
            this.Name = "MainForm";
            this.Text = "🚦 Giao Diện Giao Thông";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private ComboBox comboBox1;
        private Label label1;
        private Label label2;
        private ComboBox comboBox2;
        private DataGridView dgv1;
        private Label label3;
        private Button btnTopN;
        private Button btnDuyet;
    }
}
