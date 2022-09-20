namespace WindowsForms_packing_line
{
    partial class Countperday
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lvCountperday = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnResetCountperday = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvCountperday
            // 
            this.lvCountperday.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvCountperday.Dock = System.Windows.Forms.DockStyle.Left;
            this.lvCountperday.FullRowSelect = true;
            this.lvCountperday.GridLines = true;
            this.lvCountperday.HideSelection = false;
            this.lvCountperday.Location = new System.Drawing.Point(0, 0);
            this.lvCountperday.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.lvCountperday.Name = "lvCountperday";
            this.lvCountperday.Size = new System.Drawing.Size(800, 661);
            this.lvCountperday.TabIndex = 0;
            this.lvCountperday.UseCompatibleStateImageBehavior = false;
            this.lvCountperday.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No.";
            this.columnHeader1.Width = 61;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Kanban";
            this.columnHeader2.Width = 151;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Inner Box";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 166;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Carton Box";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 177;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Export Box";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 179;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(811, 54);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(161, 80);
            this.btnExportExcel.TabIndex = 1;
            this.btnExportExcel.Text = "Export to\r\nExcel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // btnResetCountperday
            // 
            this.btnResetCountperday.Location = new System.Drawing.Point(811, 161);
            this.btnResetCountperday.Name = "btnResetCountperday";
            this.btnResetCountperday.Size = new System.Drawing.Size(161, 105);
            this.btnResetCountperday.TabIndex = 2;
            this.btnResetCountperday.Text = "Reset\r\nCount per day";
            this.btnResetCountperday.UseVisualStyleBackColor = true;
            this.btnResetCountperday.Click += new System.EventHandler(this.btnResetCountperday_Click);
            // 
            // Countperday
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.btnResetCountperday);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.lvCountperday);
            this.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "Countperday";
            this.Text = "Countperday";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvCountperday;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button btnResetCountperday;
    }
}