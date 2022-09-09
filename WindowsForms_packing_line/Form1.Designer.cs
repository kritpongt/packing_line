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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Checker = new System.Windows.Forms.TabPage();
            this.lRemainingCarton = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lMasterExport = new System.Windows.Forms.Label();
            this.tbExportBox = new System.Windows.Forms.TextBox();
            this.lNeedExport = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lMasterCarton = new System.Windows.Forms.Label();
            this.tbCartonBox = new System.Windows.Forms.TextBox();
            this.lNeedCarton = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lMasterB = new System.Windows.Forms.Label();
            this.lIsPort2Open = new System.Windows.Forms.Label();
            this.tbInnerBoxB = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lMasterA = new System.Windows.Forms.Label();
            this.lIsPort1Open = new System.Windows.Forms.Label();
            this.tbInnerBoxA = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.lRemainingInner = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.tbQTY = new System.Windows.Forms.TextBox();
            this.tbModel = new System.Windows.Forms.TextBox();
            this.tbKanban = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Settings = new System.Windows.Forms.TabPage();
            this.btnSavePorts = new System.Windows.Forms.Button();
            this.cbPort4 = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.cbPort3 = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cbPort2 = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.cbPort1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Edit = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.lIsPort3Open = new System.Windows.Forms.Label();
            this.lIsPort4Open = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.Checker.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Settings.SuspendLayout();
            this.Edit.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(22, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operator ID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(21, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 32);
            this.label2.TabIndex = 1;
            this.label2.Text = "Operator Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(429, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 32);
            this.label3.TabIndex = 2;
            this.label3.Text = "Position:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Checker);
            this.tabControl1.Controls.Add(this.Settings);
            this.tabControl1.Controls.Add(this.Edit);
            this.tabControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 94);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1008, 635);
            this.tabControl1.TabIndex = 3;
            // 
            // Checker
            // 
            this.Checker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(52)))), ((int)(((byte)(96)))));
            this.Checker.Controls.Add(this.lRemainingCarton);
            this.Checker.Controls.Add(this.groupBox4);
            this.Checker.Controls.Add(this.groupBox3);
            this.Checker.Controls.Add(this.groupBox2);
            this.Checker.Controls.Add(this.groupBox1);
            this.Checker.Controls.Add(this.label12);
            this.Checker.Controls.Add(this.lbLog);
            this.Checker.Controls.Add(this.lRemainingInner);
            this.Checker.Controls.Add(this.btnStart);
            this.Checker.Controls.Add(this.tbQTY);
            this.Checker.Controls.Add(this.tbModel);
            this.Checker.Controls.Add(this.tbKanban);
            this.Checker.Controls.Add(this.label6);
            this.Checker.Controls.Add(this.label5);
            this.Checker.Controls.Add(this.label4);
            this.Checker.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Checker.Location = new System.Drawing.Point(4, 33);
            this.Checker.Name = "Checker";
            this.Checker.Padding = new System.Windows.Forms.Padding(3);
            this.Checker.Size = new System.Drawing.Size(1000, 598);
            this.Checker.TabIndex = 0;
            this.Checker.Text = "Checker";
            // 
            // lRemainingCarton
            // 
            this.lRemainingCarton.AutoSize = true;
            this.lRemainingCarton.Location = new System.Drawing.Point(504, 370);
            this.lRemainingCarton.Name = "lRemainingCarton";
            this.lRemainingCarton.Size = new System.Drawing.Size(70, 24);
            this.lRemainingCarton.TabIndex = 16;
            this.lRemainingCarton.Text = "Test:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lMasterExport);
            this.groupBox4.Controls.Add(this.lIsPort4Open);
            this.groupBox4.Controls.Add(this.tbExportBox);
            this.groupBox4.Controls.Add(this.lNeedExport);
            this.groupBox4.ForeColor = System.Drawing.Color.Orange;
            this.groupBox4.Location = new System.Drawing.Point(502, 397);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(490, 136);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Export Box";
            // 
            // lMasterExport
            // 
            this.lMasterExport.AutoSize = true;
            this.lMasterExport.Location = new System.Drawing.Point(6, 28);
            this.lMasterExport.Name = "lMasterExport";
            this.lMasterExport.Size = new System.Drawing.Size(118, 24);
            this.lMasterExport.TabIndex = 29;
            this.lMasterExport.Text = "Master: -";
            // 
            // tbExportBox
            // 
            this.tbExportBox.Location = new System.Drawing.Point(6, 55);
            this.tbExportBox.Name = "tbExportBox";
            this.tbExportBox.Size = new System.Drawing.Size(478, 32);
            this.tbExportBox.TabIndex = 14;
            // 
            // lNeedExport
            // 
            this.lNeedExport.AutoSize = true;
            this.lNeedExport.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lNeedExport.Location = new System.Drawing.Point(3, 109);
            this.lNeedExport.Name = "lNeedExport";
            this.lNeedExport.Size = new System.Drawing.Size(94, 24);
            this.lNeedExport.TabIndex = 20;
            this.lNeedExport.Text = "Need: 0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lMasterCarton);
            this.groupBox3.Controls.Add(this.lIsPort3Open);
            this.groupBox3.Controls.Add(this.tbCartonBox);
            this.groupBox3.Controls.Add(this.lNeedCarton);
            this.groupBox3.ForeColor = System.Drawing.Color.MediumSpringGreen;
            this.groupBox3.Location = new System.Drawing.Point(502, 231);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(490, 136);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Carton Box";
            // 
            // lMasterCarton
            // 
            this.lMasterCarton.AutoSize = true;
            this.lMasterCarton.Location = new System.Drawing.Point(6, 28);
            this.lMasterCarton.Name = "lMasterCarton";
            this.lMasterCarton.Size = new System.Drawing.Size(118, 24);
            this.lMasterCarton.TabIndex = 28;
            this.lMasterCarton.Text = "Master: -";
            // 
            // tbCartonBox
            // 
            this.tbCartonBox.Location = new System.Drawing.Point(6, 55);
            this.tbCartonBox.Name = "tbCartonBox";
            this.tbCartonBox.Size = new System.Drawing.Size(478, 32);
            this.tbCartonBox.TabIndex = 14;
            // 
            // lNeedCarton
            // 
            this.lNeedCarton.AutoSize = true;
            this.lNeedCarton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lNeedCarton.Location = new System.Drawing.Point(3, 109);
            this.lNeedCarton.Name = "lNeedCarton";
            this.lNeedCarton.Size = new System.Drawing.Size(94, 24);
            this.lNeedCarton.TabIndex = 19;
            this.lNeedCarton.Text = "Need: 0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lMasterB);
            this.groupBox2.Controls.Add(this.lIsPort2Open);
            this.groupBox2.Controls.Add(this.tbInnerBoxB);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox2.Location = new System.Drawing.Point(502, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(490, 136);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inner Box B";
            // 
            // lMasterB
            // 
            this.lMasterB.AutoSize = true;
            this.lMasterB.Location = new System.Drawing.Point(6, 93);
            this.lMasterB.Name = "lMasterB";
            this.lMasterB.Size = new System.Drawing.Size(118, 24);
            this.lMasterB.TabIndex = 27;
            this.lMasterB.Text = "Master: -";
            // 
            // lIsPort2Open
            // 
            this.lIsPort2Open.AutoSize = true;
            this.lIsPort2Open.BackColor = System.Drawing.Color.Crimson;
            this.lIsPort2Open.Dock = System.Windows.Forms.DockStyle.Right;
            this.lIsPort2Open.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lIsPort2Open.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lIsPort2Open.Location = new System.Drawing.Point(367, 28);
            this.lIsPort2Open.Name = "lIsPort2Open";
            this.lIsPort2Open.Size = new System.Drawing.Size(120, 18);
            this.lIsPort2Open.TabIndex = 26;
            this.lIsPort2Open.Text = "Port2: Offline";
            // 
            // tbInnerBoxB
            // 
            this.tbInnerBoxB.Location = new System.Drawing.Point(6, 49);
            this.tbInnerBoxB.Name = "tbInnerBoxB";
            this.tbInnerBoxB.Size = new System.Drawing.Size(478, 32);
            this.tbInnerBoxB.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lMasterA);
            this.groupBox1.Controls.Add(this.lIsPort1Open);
            this.groupBox1.Controls.Add(this.tbInnerBoxA);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.groupBox1.Location = new System.Drawing.Point(8, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 136);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inner Box A";
            // 
            // lMasterA
            // 
            this.lMasterA.AutoSize = true;
            this.lMasterA.Location = new System.Drawing.Point(11, 93);
            this.lMasterA.Name = "lMasterA";
            this.lMasterA.Size = new System.Drawing.Size(118, 24);
            this.lMasterA.TabIndex = 26;
            this.lMasterA.Text = "Master: -";
            // 
            // lIsPort1Open
            // 
            this.lIsPort1Open.AutoSize = true;
            this.lIsPort1Open.BackColor = System.Drawing.Color.Crimson;
            this.lIsPort1Open.Dock = System.Windows.Forms.DockStyle.Right;
            this.lIsPort1Open.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lIsPort1Open.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lIsPort1Open.Location = new System.Drawing.Point(367, 28);
            this.lIsPort1Open.Name = "lIsPort1Open";
            this.lIsPort1Open.Size = new System.Drawing.Size(120, 18);
            this.lIsPort1Open.TabIndex = 25;
            this.lIsPort1Open.Text = "Port1: Offline";
            // 
            // tbInnerBoxA
            // 
            this.tbInnerBoxA.Location = new System.Drawing.Point(6, 49);
            this.tbInnerBoxA.Name = "tbInnerBoxA";
            this.tbInnerBoxA.Size = new System.Drawing.Size(478, 32);
            this.tbInnerBoxA.TabIndex = 13;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(40, 204);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 24);
            this.label12.TabIndex = 18;
            this.label12.Text = "Log:";
            // 
            // lbLog
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 24;
            this.lbLog.Location = new System.Drawing.Point(38, 231);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(428, 340);
            this.lbLog.TabIndex = 17;
            // 
            // lRemainingInner
            // 
            this.lRemainingInner.AutoSize = true;
            this.lRemainingInner.Location = new System.Drawing.Point(504, 204);
            this.lRemainingInner.Name = "lRemainingInner";
            this.lRemainingInner.Size = new System.Drawing.Size(70, 24);
            this.lRemainingInner.TabIndex = 15;
            this.lRemainingInner.Text = "Test:";
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.MediumAquamarine;
            this.btnStart.Location = new System.Drawing.Point(872, 6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(120, 50);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tbQTY
            // 
            this.tbQTY.Location = new System.Drawing.Point(743, 16);
            this.tbQTY.Name = "tbQTY";
            this.tbQTY.Size = new System.Drawing.Size(109, 32);
            this.tbQTY.TabIndex = 9;
            // 
            // tbModel
            // 
            this.tbModel.Enabled = false;
            this.tbModel.Location = new System.Drawing.Point(402, 16);
            this.tbModel.Name = "tbModel";
            this.tbModel.Size = new System.Drawing.Size(220, 32);
            this.tbModel.TabIndex = 8;
            // 
            // tbKanban
            // 
            this.tbKanban.Location = new System.Drawing.Point(83, 17);
            this.tbKanban.Name = "tbKanban";
            this.tbKanban.Size = new System.Drawing.Size(220, 32);
            this.tbKanban.TabIndex = 7;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(679, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 24);
            this.label6.TabIndex = 6;
            this.label6.Text = "QTY:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(314, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 24);
            this.label5.TabIndex = 5;
            this.label5.Text = "Model:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(19, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 24);
            this.label4.TabIndex = 4;
            this.label4.Text = "K/B:";
            // 
            // Settings
            // 
            this.Settings.Controls.Add(this.btnSavePorts);
            this.Settings.Controls.Add(this.cbPort4);
            this.Settings.Controls.Add(this.label19);
            this.Settings.Controls.Add(this.cbPort3);
            this.Settings.Controls.Add(this.label18);
            this.Settings.Controls.Add(this.cbPort2);
            this.Settings.Controls.Add(this.label17);
            this.Settings.Controls.Add(this.cbPort1);
            this.Settings.Controls.Add(this.label7);
            this.Settings.Location = new System.Drawing.Point(4, 33);
            this.Settings.Name = "Settings";
            this.Settings.Padding = new System.Windows.Forms.Padding(3);
            this.Settings.Size = new System.Drawing.Size(1000, 598);
            this.Settings.TabIndex = 1;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // btnSavePorts
            // 
            this.btnSavePorts.Location = new System.Drawing.Point(246, 185);
            this.btnSavePorts.Name = "btnSavePorts";
            this.btnSavePorts.Size = new System.Drawing.Size(121, 44);
            this.btnSavePorts.TabIndex = 2;
            this.btnSavePorts.Text = "Save";
            this.btnSavePorts.UseVisualStyleBackColor = true;
            this.btnSavePorts.Click += new System.EventHandler(this.btnSavePorts_Click);
            // 
            // cbPort4
            // 
            this.cbPort4.FormattingEnabled = true;
            this.cbPort4.Location = new System.Drawing.Point(103, 147);
            this.cbPort4.Name = "cbPort4";
            this.cbPort4.Size = new System.Drawing.Size(264, 32);
            this.cbPort4.TabIndex = 1;
            this.cbPort4.SelectedIndexChanged += new System.EventHandler(this.cbPort4_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(15, 150);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(82, 24);
            this.label19.TabIndex = 0;
            this.label19.Text = "Port4:";
            // 
            // cbPort3
            // 
            this.cbPort3.FormattingEnabled = true;
            this.cbPort3.Location = new System.Drawing.Point(103, 109);
            this.cbPort3.Name = "cbPort3";
            this.cbPort3.Size = new System.Drawing.Size(264, 32);
            this.cbPort3.TabIndex = 1;
            this.cbPort3.SelectedIndexChanged += new System.EventHandler(this.cbPort3_SelectedIndexChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(15, 112);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(82, 24);
            this.label18.TabIndex = 0;
            this.label18.Text = "Port3:";
            // 
            // cbPort2
            // 
            this.cbPort2.FormattingEnabled = true;
            this.cbPort2.Location = new System.Drawing.Point(103, 71);
            this.cbPort2.Name = "cbPort2";
            this.cbPort2.Size = new System.Drawing.Size(264, 32);
            this.cbPort2.TabIndex = 1;
            this.cbPort2.SelectedIndexChanged += new System.EventHandler(this.cbPort2_SelectedIndexChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(15, 74);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(82, 24);
            this.label17.TabIndex = 0;
            this.label17.Text = "Port2:";
            // 
            // cbPort1
            // 
            this.cbPort1.Location = new System.Drawing.Point(103, 33);
            this.cbPort1.Name = "cbPort1";
            this.cbPort1.Size = new System.Drawing.Size(264, 32);
            this.cbPort1.TabIndex = 1;
            this.cbPort1.SelectedIndexChanged += new System.EventHandler(this.cbPort1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 24);
            this.label7.TabIndex = 0;
            this.label7.Text = "Port1:";
            // 
            // Edit
            // 
            this.Edit.Controls.Add(this.groupBox5);
            this.Edit.Location = new System.Drawing.Point(4, 33);
            this.Edit.Name = "Edit";
            this.Edit.Size = new System.Drawing.Size(1000, 598);
            this.Edit.TabIndex = 2;
            this.Edit.Text = "Edit Database";
            this.Edit.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Location = new System.Drawing.Point(8, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(984, 91);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Search";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(16, 31);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(946, 32);
            this.textBox1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(33)))), ((int)(((byte)(62)))));
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1008, 100);
            this.panel1.TabIndex = 4;
            // 
            // lIsPort3Open
            // 
            this.lIsPort3Open.AutoSize = true;
            this.lIsPort3Open.BackColor = System.Drawing.Color.Crimson;
            this.lIsPort3Open.Dock = System.Windows.Forms.DockStyle.Right;
            this.lIsPort3Open.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lIsPort3Open.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lIsPort3Open.Location = new System.Drawing.Point(367, 28);
            this.lIsPort3Open.Name = "lIsPort3Open";
            this.lIsPort3Open.Size = new System.Drawing.Size(120, 18);
            this.lIsPort3Open.TabIndex = 26;
            this.lIsPort3Open.Text = "Port3: Offline";
            // 
            // lIsPort4Open
            // 
            this.lIsPort4Open.AutoSize = true;
            this.lIsPort4Open.BackColor = System.Drawing.Color.Crimson;
            this.lIsPort4Open.Dock = System.Windows.Forms.DockStyle.Right;
            this.lIsPort4Open.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lIsPort4Open.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lIsPort4Open.Location = new System.Drawing.Point(367, 28);
            this.lIsPort4Open.Name = "lIsPort4Open";
            this.lIsPort4Open.Size = new System.Drawing.Size(120, 18);
            this.lIsPort4Open.TabIndex = 26;
            this.lIsPort4Open.Text = "Port4: Offline";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.Checker.ResumeLayout(false);
            this.Checker.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            this.Edit.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage Checker;
        private System.Windows.Forms.TabPage Settings;
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
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label lRemainingCarton;
        private System.Windows.Forms.TextBox tbExportBox;
        private System.Windows.Forms.TextBox tbCartonBox;
        private System.Windows.Forms.ComboBox cbPort4;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.ComboBox cbPort3;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cbPort2;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button btnSavePorts;
        private System.Windows.Forms.Label lNeedExport;
        private System.Windows.Forms.Label lNeedCarton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lIsPort2Open;
        private System.Windows.Forms.Label lIsPort1Open;
        private System.Windows.Forms.TabPage Edit;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lMasterA;
        private System.Windows.Forms.Label lMasterB;
        private System.Windows.Forms.Label lMasterExport;
        private System.Windows.Forms.Label lMasterCarton;
        private System.Windows.Forms.Label lIsPort4Open;
        private System.Windows.Forms.Label lIsPort3Open;
    }
}

