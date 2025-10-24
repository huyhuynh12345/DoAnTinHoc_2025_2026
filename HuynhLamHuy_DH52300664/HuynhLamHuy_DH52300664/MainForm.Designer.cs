using System;
using System.Windows.Forms;

namespace HuynhLamHuy_DH52300664
{
    partial class MainForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnGhiKetQua;
        private DataGridView dgv1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnGhiKetQua = new System.Windows.Forms.Button();
            this.dgv1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGhiKetQua
            // 
            this.btnGhiKetQua.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGhiKetQua.Location = new System.Drawing.Point(20, 20);
            this.btnGhiKetQua.Name = "btnGhiKetQua";
            this.btnGhiKetQua.Size = new System.Drawing.Size(150, 40);
            this.btnGhiKetQua.TabIndex = 0;
            this.btnGhiKetQua.Text = "Ghi Kết Quả";
            this.btnGhiKetQua.UseVisualStyleBackColor = true;
            this.btnGhiKetQua.Click += new System.EventHandler(this.btnGhiKetQua_Click);
            // 
            // dgv1
            // 
            this.dgv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgv1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv1.Location = new System.Drawing.Point(20, 80);
            this.dgv1.Name = "dgv1";
            this.dgv1.ReadOnly = true;
            this.dgv1.Size = new System.Drawing.Size(950, 500);
            this.dgv1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgv1);
            this.Controls.Add(this.btnGhiKetQua);
            this.Name = "MainForm";
            this.Text = "🚦 Giao Diện Giao Thông";
            ((System.ComponentModel.ISupportInitialize)(this.dgv1)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
