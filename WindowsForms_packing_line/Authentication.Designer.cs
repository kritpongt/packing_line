namespace WindowsForms_packing_line
{
    partial class Authentication
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
            this.tbAlarm = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnRFID = new System.Windows.Forms.Button();
            this.btnX = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(98, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(469, 78);
            this.label1.TabIndex = 0;
            this.label1.Text = "Login to Reset Alarm\r\nBy Administrator / Supervisor";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbAlarm
            // 
            this.tbAlarm.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbAlarm.Location = new System.Drawing.Point(195, 179);
            this.tbAlarm.Name = "tbAlarm";
            this.tbAlarm.PasswordChar = '*';
            this.tbAlarm.Size = new System.Drawing.Size(280, 41);
            this.tbAlarm.TabIndex = 1;
            this.tbAlarm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbAlarm.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAlarm_KeyDown);
            // 
            // btnLogin
            // 
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.SystemColors.Control;
            this.btnLogin.Location = new System.Drawing.Point(195, 238);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(125, 53);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnRFID
            // 
            this.btnRFID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRFID.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRFID.ForeColor = System.Drawing.SystemColors.Control;
            this.btnRFID.Location = new System.Drawing.Point(350, 238);
            this.btnRFID.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.btnRFID.Name = "btnRFID";
            this.btnRFID.Size = new System.Drawing.Size(125, 53);
            this.btnRFID.TabIndex = 4;
            this.btnRFID.Text = "RFID";
            this.btnRFID.UseVisualStyleBackColor = true;
            this.btnRFID.Click += new System.EventHandler(this.btnRFID_Click);
            // 
            // btnX
            // 
            this.btnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnX.Font = new System.Drawing.Font("Consolas", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnX.ForeColor = System.Drawing.SystemColors.Control;
            this.btnX.Location = new System.Drawing.Point(602, 12);
            this.btnX.Name = "btnX";
            this.btnX.Size = new System.Drawing.Size(50, 50);
            this.btnX.TabIndex = 4;
            this.btnX.Text = "X";
            this.btnX.UseVisualStyleBackColor = true;
            this.btnX.Click += new System.EventHandler(this.btnX_Click);
            // 
            // Authentication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.ClientSize = new System.Drawing.Size(664, 411);
            this.Controls.Add(this.btnX);
            this.Controls.Add(this.btnRFID);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.tbAlarm);
            this.Controls.Add(this.label1);
            this.Name = "Authentication";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Authentication";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAlarm;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnRFID;
        private System.Windows.Forms.Button btnX;
    }
}