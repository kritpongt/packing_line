namespace WindowsForms_packing_line
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.lRemainingExport = new System.Windows.Forms.Label();
            this.lRemainingCarton = new System.Windows.Forms.Label();
            this.lRemainingInner = new System.Windows.Forms.Label();
            this.tbExportBox = new System.Windows.Forms.TextBox();
            this.tbCartonBox = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbInnerBoxB = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbInnerBoxA = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.tbQTY = new System.Windows.Forms.TextBox();
            this.tbModel = new System.Windows.Forms.TextBox();
            this.tbKanban = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSavePorts = new System.Windows.Forms.Button();
            this.cbPort4 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.cbPort3 = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cbPort2 = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbPort1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operator ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Operator Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Position:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 67);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 357);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.lbLog);
            this.tabPage1.Controls.Add(this.lRemainingExport);
            this.tabPage1.Controls.Add(this.lRemainingCarton);
            this.tabPage1.Controls.Add(this.lRemainingInner);
            this.tabPage1.Controls.Add(this.tbExportBox);
            this.tabPage1.Controls.Add(this.tbCartonBox);
            this.tabPage1.Controls.Add(this.label15);
            this.tabPage1.Controls.Add(this.tbInnerBoxB);
            this.tabPage1.Controls.Add(this.label13);
            this.tabPage1.Controls.Add(this.tbInnerBoxA);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.btnStart);
            this.tabPage1.Controls.Add(this.tbQTY);
            this.tabPage1.Controls.Add(this.tbModel);
            this.tabPage1.Controls.Add(this.tbKanban);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(768, 331);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Checker";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 106);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Log:";
            // 
            // lbLog
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.Location = new System.Drawing.Point(22, 122);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(378, 199);
            this.lbLog.TabIndex = 17;
            // 
            // lRemainingExport
            // 
            this.lRemainingExport.AutoSize = true;
            this.lRemainingExport.Location = new System.Drawing.Point(536, 202);
            this.lRemainingExport.Name = "lRemainingExport";
            this.lRemainingExport.Size = new System.Drawing.Size(60, 13);
            this.lRemainingExport.TabIndex = 16;
            this.lRemainingExport.Text = "Remaining:";
            // 
            // lRemainingCarton
            // 
            this.lRemainingCarton.AutoSize = true;
            this.lRemainingCarton.Location = new System.Drawing.Point(536, 122);
            this.lRemainingCarton.Name = "lRemainingCarton";
            this.lRemainingCarton.Size = new System.Drawing.Size(60, 13);
            this.lRemainingCarton.TabIndex = 16;
            this.lRemainingCarton.Text = "Remaining:";
            // 
            // lRemainingInner
            // 
            this.lRemainingInner.AutoSize = true;
            this.lRemainingInner.Location = new System.Drawing.Point(243, 77);
            this.lRemainingInner.Name = "lRemainingInner";
            this.lRemainingInner.Size = new System.Drawing.Size(60, 13);
            this.lRemainingInner.TabIndex = 15;
            this.lRemainingInner.Text = "Remaining:";
            // 
            // tbExportBox
            // 
            this.tbExportBox.Location = new System.Drawing.Point(430, 202);
            this.tbExportBox.Name = "tbExportBox";
            this.tbExportBox.Size = new System.Drawing.Size(100, 20);
            this.tbExportBox.TabIndex = 14;
            // 
            // tbCartonBox
            // 
            this.tbCartonBox.Location = new System.Drawing.Point(430, 122);
            this.tbCartonBox.Name = "tbCartonBox";
            this.tbCartonBox.Size = new System.Drawing.Size(100, 20);
            this.tbCartonBox.TabIndex = 14;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(427, 186);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 12;
            this.label15.Text = "Export Box:";
            // 
            // tbInnerBoxB
            // 
            this.tbInnerBoxB.Location = new System.Drawing.Point(137, 77);
            this.tbInnerBoxB.Name = "tbInnerBoxB";
            this.tbInnerBoxB.Size = new System.Drawing.Size(100, 20);
            this.tbInnerBoxB.TabIndex = 14;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(427, 106);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "Carton Box:";
            // 
            // tbInnerBoxA
            // 
            this.tbInnerBoxA.Location = new System.Drawing.Point(22, 77);
            this.tbInnerBoxA.Name = "tbInnerBoxA";
            this.tbInnerBoxA.Size = new System.Drawing.Size(100, 20);
            this.tbInnerBoxA.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(134, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Inner Box B:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Inner Box A:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(455, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tbQTY
            // 
            this.tbQTY.Location = new System.Drawing.Point(349, 15);
            this.tbQTY.Name = "tbQTY";
            this.tbQTY.Size = new System.Drawing.Size(100, 20);
            this.tbQTY.TabIndex = 9;
            // 
            // tbModel
            // 
            this.tbModel.Location = new System.Drawing.Point(205, 15);
            this.tbModel.Name = "tbModel";
            this.tbModel.Size = new System.Drawing.Size(100, 20);
            this.tbModel.TabIndex = 8;
            // 
            // tbKanban
            // 
            this.tbKanban.Location = new System.Drawing.Point(54, 15);
            this.tbKanban.Name = "tbKanban";
            this.tbKanban.Size = new System.Drawing.Size(100, 20);
            this.tbKanban.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(311, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "QTY:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Model:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "K/B:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnSavePorts);
            this.tabPage2.Controls.Add(this.cbPort4);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.cbPort3);
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.cbPort2);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.cbPort1);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(768, 331);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSavePorts
            // 
            this.btnSavePorts.Location = new System.Drawing.Point(101, 139);
            this.btnSavePorts.Name = "btnSavePorts";
            this.btnSavePorts.Size = new System.Drawing.Size(75, 23);
            this.btnSavePorts.TabIndex = 2;
            this.btnSavePorts.Text = "Save";
            this.btnSavePorts.UseVisualStyleBackColor = true;
            this.btnSavePorts.Click += new System.EventHandler(this.btnSavePorts_Click);
            // 
            // cbPort4
            // 
            this.cbPort4.FormattingEnabled = true;
            this.cbPort4.Location = new System.Drawing.Point(56, 111);
            this.cbPort4.Name = "cbPort4";
            this.cbPort4.Size = new System.Drawing.Size(121, 21);
            this.cbPort4.TabIndex = 1;
            this.cbPort4.SelectedIndexChanged += new System.EventHandler(this.cbPort4_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 114);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(35, 13);
            this.label19.TabIndex = 0;
            this.label19.Text = "Port4:";
            // 
            // cbPort3
            // 
            this.cbPort3.FormattingEnabled = true;
            this.cbPort3.Location = new System.Drawing.Point(56, 84);
            this.cbPort3.Name = "cbPort3";
            this.cbPort3.Size = new System.Drawing.Size(121, 21);
            this.cbPort3.TabIndex = 1;
            this.cbPort3.SelectedIndexChanged += new System.EventHandler(this.cbPort3_SelectedIndexChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 87);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(35, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "Port3:";
            // 
            // cbPort2
            // 
            this.cbPort2.FormattingEnabled = true;
            this.cbPort2.Location = new System.Drawing.Point(56, 57);
            this.cbPort2.Name = "cbPort2";
            this.cbPort2.Size = new System.Drawing.Size(121, 21);
            this.cbPort2.TabIndex = 1;
            this.cbPort2.SelectedIndexChanged += new System.EventHandler(this.cbPort2_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(15, 60);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(35, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Port2:";
            // 
            // cbPort1
            // 
            this.cbPort1.Location = new System.Drawing.Point(56, 30);
            this.cbPort1.Name = "cbPort1";
            this.cbPort1.Size = new System.Drawing.Size(121, 21);
            this.cbPort1.TabIndex = 1;
            this.cbPort1.SelectedIndexChanged += new System.EventHandler(this.cbPort1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Port1:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox tbQTY;
        private System.Windows.Forms.TextBox tbModel;
        private System.Windows.Forms.TextBox tbKanban;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbPort1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lRemainingInner;
        private System.Windows.Forms.TextBox tbInnerBoxB;
        private System.Windows.Forms.TextBox tbInnerBoxA;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label lRemainingExport;
        private System.Windows.Forms.Label lRemainingCarton;
        private System.Windows.Forms.TextBox tbExportBox;
        private System.Windows.Forms.TextBox tbCartonBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cbPort4;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cbPort3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cbPort2;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnSavePorts;
    }
}

