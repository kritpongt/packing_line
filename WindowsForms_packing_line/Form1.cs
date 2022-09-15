using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Runtime.Remoting.Messaging;
using System.Linq.Expressions;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Windows.Documents;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Cryptography;
using System.Data.Common;
using System.Diagnostics.Eventing.Reader;

namespace WindowsForms_packing_line
{
    public partial class Form1 : Form
    {
        private SerialPort port1;
        private SerialPort port2;
        private SerialPort port3;
        private SerialPort port4;
        private SerialPort portRFID = new SerialPort("COM7", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        string connectStr = "server=" + WindowsForms_packing_line.Properties.Settings.Default.dbIPServer + ";port=3306;Database=packing_line_element;uid=root;pwd=;SslMode=none;";
        private int qty, innerbox_max, cartonbox_max;   //get from db
        int inner_scanned = 0;  //if inner is scanned +1
        int inner_count = 0, carton_count = 0, export_count, carton_need = 0, export_need = 0; //counter +1 the larger box
        string inner_a_master;  //No. inner master
        string inner_b_master;  //No. inner master
        string carton_master;   //No. carton master
        string export_master;   //No. export master
        string selected_kanban_id = ""; //temp_str kanban for update database
        string selected_account_tagpass = ""; //temp_str account for update database
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            refreshListViewMaster();
            refreshListViewAccount();
            initialPorts();
            this.ActiveControl = tbLogin;
        }
        void lbLog_SetColor(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                Brush brush = Brushes.Red;
                e.Graphics.DrawString(((ListBox)sender).Items[e.Index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Ports Indicator(in ComboBoxes)
            string[] getPorts = SerialPort.GetPortNames();
            cbPort1.Items.AddRange(getPorts);
            cbPort2.Items.AddRange(getPorts);
            cbPort3.Items.AddRange(getPorts);
            cbPort4.Items.AddRange(getPorts);
        }
        //Login
        private void tbLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string TABLE = "test_account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE OperatorID = '" + tbLogin.Text + "';";
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataReader reader;
                dbcommand.CommandTimeout = 100;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
                    while (reader.Read())
                    {
                        lOperatorID.Text = "Operator ID: " + reader.GetString("OperatorID");
                        lOperatorName.Text = "Operator Name: " + reader.GetString("Name") + " " + reader.GetString("Surname");
                        lPosition.Text = "Position: " + reader.GetString("Position");
                        pLogin.Hide();
                        roleChecker(reader.GetString("Position"));
                        tbKanban.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string TABLE = "test_account";
            string queryList = "SELECT * FROM " + TABLE + " WHERE OperatorID = '" + tbLogin.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 100;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    lOperatorID.Text = "Operator ID: " + reader.GetString("OperatorID");
                    lOperatorName.Text = "Operator Name: " + reader.GetString("Name") + " " + reader.GetString("Surname");
                    lPosition.Text = "Position: " + reader.GetString("Position");
                    pLogin.Hide();
                    roleChecker(reader.GetString("Position"));
                    tbKanban.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        private void pIcon_Click(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }//OK
        //IF ports are selected then settings
        private void cbPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPort1.Text == cbPort2.Text)
            {
                cbPort2.SelectedIndex = -1;
            }
            else if (cbPort1.Text == cbPort3.Text)
            {
                cbPort3.SelectedIndex = -1;
            }
            else if (cbPort1.Text == cbPort4.Text)
            {
                cbPort4.SelectedIndex = -1;
            }
        }//OK
        private void cbPort2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPort2.Text == cbPort1.Text)
            {
                cbPort1.SelectedIndex = -1;
            }
            else if (cbPort2.Text == cbPort3.Text)
            {
                cbPort3.SelectedIndex = -1;
            }
            else if (cbPort2.Text == cbPort4.Text)
            {
                cbPort4.SelectedIndex = -1;
            }
        }//OK
        private void cbPort3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPort3.Text == cbPort1.Text)
            {
                cbPort1.SelectedIndex = -1;
            }
            else if (cbPort3.Text == cbPort2.Text)
            {
                cbPort2.SelectedIndex = -1;
            }
            else if (cbPort3.Text == cbPort4.Text)
            {
                cbPort4.SelectedIndex = -1;
            }
        }//OK
        private void cbPort4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPort4.Text == cbPort1.Text)
            {
                cbPort1.SelectedIndex = -1;
            }
            else if (cbPort4.Text == cbPort2.Text)
            {
                cbPort2.SelectedIndex = -1;
            }
            else if (cbPort4.Text == cbPort3.Text)
            {
                cbPort3.SelectedIndex = -1;
            }
        }//OK
        //Set Baud rate
        private void cbBaudrate1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbBaudrate1.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Baudrate1 = int.Parse(cbBaudrate1.Text);
            }
        }//OK
        private void cbBaudrate2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBaudrate2.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Baudrate2 = int.Parse(cbBaudrate2.Text);
            }
        }//OK
        private void cbBaudrate3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBaudrate3.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Baudrate3 = int.Parse(cbBaudrate3.Text);
            }
        }//OK
        private void cbBaudrate4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBaudrate4.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Baudrate4 = int.Parse(cbBaudrate4.Text);
                WindowsForms_packing_line.Properties.Settings.Default.Save();
            }
        }//OK
        //Set Parity bit
        private void cbParitybit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbParitybit1.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Paritybit1 = (Parity)Enum.Parse(typeof(Parity), cbParitybit1.Text);
            }
        }//OK
        private void cbParitybit2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbParitybit2.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Paritybit2 = (Parity)Enum.Parse(typeof(Parity), cbParitybit2.Text);
            }
        }//OK
        private void cbParitybit3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbParitybit3.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Paritybit3 = (Parity)Enum.Parse(typeof(Parity), cbParitybit3.Text);
            }
        }//OK
        private void cbParitybit4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbParitybit4.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Paritybit4 = (Parity)Enum.Parse(typeof(Parity), cbParitybit4.Text);
            }
        }//OK
        //Set Data size
        private void cbDatasize1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDatasize1.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Datasize1 = int.Parse(cbDatasize1.Text);
            }
        }//OK
        private void cbDatasize2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDatasize2.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Datasize2 = int.Parse(cbDatasize2.Text);
            }
        }//OK
        private void cbDatasize3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDatasize3.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Datasize3 = int.Parse(cbDatasize3.Text);
            }
        }//OK
        private void cbDatasize4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDatasize4.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Datasize4 = int.Parse(cbDatasize4.Text);
            }
        }//OK
        //Set Stop bits
        private void cbStopbits1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( cbStopbits1.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Stopbits1 = (StopBits)Enum.Parse(typeof(StopBits), cbStopbits1.Text);
            }
        }//OK
        private void cbStopbits2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStopbits2.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Stopbits2 = (StopBits)Enum.Parse(typeof(StopBits), cbStopbits2.Text);
            }
        }//OK
        private void cbStopbits3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStopbits3.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Stopbits3 = (StopBits)Enum.Parse(typeof(StopBits), cbStopbits3.Text);
            }
        }//OK
        private void cbStopbits4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStopbits4.Text != "")
            {
                WindowsForms_packing_line.Properties.Settings.Default.Stopbits4 = (StopBits)Enum.Parse(typeof(StopBits), cbStopbits4.Text);
            }
        }//OK
        //Button Save Ports
        private void btnSavePorts_Click(object sender, EventArgs e)
        {
            WindowsForms_packing_line.Properties.Settings.Default.Port1 = "";
            WindowsForms_packing_line.Properties.Settings.Default.Port2 = "";
            WindowsForms_packing_line.Properties.Settings.Default.Port3 = "";
            WindowsForms_packing_line.Properties.Settings.Default.Port4 = "";
            WindowsForms_packing_line.Properties.Settings.Default.Port1 = cbPort1.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port2 = cbPort2.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port3 = cbPort3.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port4 = cbPort4.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Save();
            portsCloser();
            portsOpener();
        }//OK
        //Data Receiver and Output
        private void tbKanban_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string TABLE = "test_model_master";
                string queryList = "SELECT * FROM " + TABLE + " WHERE Kanban = '" + tbKanban.Text + "';";
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataReader reader;
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
                    while (reader.Read())
                    {
                        tbModel.Text = reader.GetString("ModelNo"); //Get Model
                    }
                    string[] kanban_array = tbKanban.Text.Split('|');
                    tbQTY.Text = kanban_array[1];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
            }
        }//Waitfix
        //Start Button
        private void btnStart_Click(object sender, EventArgs e)
        {
            //input qty(Integer)
            bool isInteger = false;
            try
            {
                isInteger = int.TryParse(tbQTY.Text, out qty);
                if (!isInteger)
                {
                    MessageBox.Show("Error: K/B is empty or QTY is incorrect!", "Checker");
                }
                else
                {
                    qeuryMax();
                    queryInnerA();
                    queryInnerB();
                    queryCarton();
                    queryExport();
                    inner_scanned = 0;
                    lTotal.Text = "Total: " + qty.ToString();
                    inner_count = 0;
                    carton_count = 0;
                    carton_need = 0;
                    export_need = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }//Waittest
        private void dataReceiver1(object sender, SerialDataReceivedEventArgs e)    //Receiver Inner Box A
        {
            try
            {
                string input_value = port1.ReadExisting();
                Thread.Sleep(100);
                Invoke((MethodInvoker)delegate { tbInnerBoxA.Text = input_value; });
                inner_a_master = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                if (input_value.Equals(inner_a_master))
                {
                    //log
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //inner scenned
                    if(qty - inner_scanned > 0)
                    {
                        inner_scanned++;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty - inner_scanned).ToString(); });

                        inner_count++;
                        if (inner_count == innerbox_max)
                        {
                            inner_count = 0;
                            carton_need++;
                            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need.ToString(); });
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Alarm Scan error!");
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiver2(object sender, SerialDataReceivedEventArgs e)    //Receiver Inner Box B
        {
            try
            {
                string input_value = port2.ReadExisting();
                Thread.Sleep(100);
                Invoke((MethodInvoker)delegate { tbInnerBoxB.Text = input_value; });
                inner_b_master = WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster;
                if (input_value.Equals(inner_b_master))
                {
                    //Log
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //inner scanned
                    if (qty - inner_scanned > 0)
                    {
                        inner_scanned++;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty - inner_scanned).ToString(); });

                        inner_count++;
                        if (inner_count == innerbox_max)
                        {
                            inner_count = 0;
                            carton_need++;
                            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need.ToString(); });
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Alarm Scan error!");
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiver3(object sender, SerialDataReceivedEventArgs e)    //Receiver Carton Box
        {
            try
            {
                string input_value = port3.ReadExisting();
                Thread.Sleep(100);
                Invoke((MethodInvoker)delegate { tbCartonBox.Text = input_value; });
                carton_master = WindowsForms_packing_line.Properties.Settings.Default.CartonMaster;
                if (input_value.Equals(carton_master))
                {
                    //Log
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tCarton Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    
                    if (carton_need > 0)
                    {
                        carton_need--;
                        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; }); //Decrease carton Need:

                        carton_count++;
                        if (carton_count == cartonbox_max)
                        {
                            carton_count = 0;
                            export_need++;
                            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                        }
                    }
                    else if (carton_need == 0)
                    {
                        MessageBox.Show("Alarm when Carton Box = 0");
                    }
                }
                else
                {
                    MessageBox.Show("Alarm Scan error!");
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tCarton Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiver4(object sender, SerialDataReceivedEventArgs e)    //Receiver Export Box
        {
            try
            {
                string input_value = port4.ReadExisting();
                Thread.Sleep(100);
                Invoke((MethodInvoker)delegate { tbExportBox.Text = input_value; lbLog.Items.Add(input_value + "\t\t\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                export_master = WindowsForms_packing_line.Properties.Settings.Default.ExportMaster;
                if (input_value.Equals(export_master))
                {
                    //Log
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });

                    export_need--;
                    Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; }); //Decrease export Need:
                    if (inner_scanned == 0)
                    {
                        MessageBox.Show("Alarm reset kanban!");
                    }
                }
                else
                {
                    MessageBox.Show("Alarm Scan error!");
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\t\t\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Close and Open port 1-4 Checker page
        private void lIsPort1Open_Click(object sender, EventArgs e)
        {
            try
            {
                if(port1 != null)
                {
                    if (port1.IsOpen)
                    {
                        port1.Close();
                        lIsPort1Open.Text = "Port1: Offine";
                        lIsPort1Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port1.IsOpen == false)
                    {
                        port1.Open();
                        lIsPort1Open.Text = "Port1: Online";
                        lIsPort1Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    MessageBox.Show("Port1 hasn't been selected!", "Ports Setting");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        private void lIsPort2Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (port2 != null)
                {
                    if (port2.IsOpen)
                    {
                        port2.Close();
                        lIsPort2Open.Text = "Port2: Offine";
                        lIsPort2Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port2.IsOpen == false)
                    {
                        port2.Open();
                        lIsPort2Open.Text = "Port2: Online";
                        lIsPort2Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    MessageBox.Show("Port2 hasn't been selected!", "Ports Setting");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        private void lIsPort3Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (port3 != null)
                {
                    if (port3.IsOpen)
                    {
                        port3.Close();
                        lIsPort3Open.Text = "Port3: Offine";
                        lIsPort3Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port3.IsOpen == false)
                    {
                        port3.Open();
                        lIsPort3Open.Text = "Port3: Online";
                        lIsPort3Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    MessageBox.Show("Port3 hasn't been selected!", "Ports Setting");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        private void lIsPort4Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (port4 != null)
                {
                    if (port4.IsOpen)
                    {
                        port4.Close();
                        lIsPort4Open.Text = "Port4: Offine";
                        lIsPort4Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port4.IsOpen == false)
                    {
                        port4.Open();
                        lIsPort4Open.Text = "Port4: Online";
                        lIsPort4Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    MessageBox.Show("Port4 hasn't been selected!", "Ports Setting");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        //Decrease a counter -1 at a time
        private void btnDecreaseInnerA_Click(object sender, EventArgs e)
        {
            if (inner_count>0)
            {
                inner_count--;
                lbLog.ForeColor = Color.Red;
                Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\t\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
            }
        }
        private void btnDecreaseInnerB_Click(object sender, EventArgs e)
        {
            if (inner_count > 0)
            {
                inner_count--;
                lbLog.ForeColor= Color.Red;
                Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\t\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
            }
        }
        //Edit Master page
        //Create kanban button
        private void btnCreateMaster_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Are you sure to insert new tool setting?", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                string TABLE = "test_model_master";
                string INSERT_STR = " (Kanban, ModelNo, InnerA, InnerB, Carton, Export, InnerMax, CartonMax) VALUES('" + tbKBSearch.Text + "', '" + tbDBModel.Text + "', '" + tbDBInnerA.Text + "', '" + tbDBInnerB.Text + "', '" + tbDBCarton.Text + "', '" + tbDBExport.Text + "', '" + tbDBInnerMax.Text + "', '" + tbDBCartonMax.Text + "');";
                string queryList = "INSERT INTO " + TABLE + INSERT_STR;
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataReader reader;
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
                    MessageBox.Show("Insert Success", "Dababase");
                    tbEditMasterClear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
            }
        }
        //Update kanban button
        private void btnUpdateMaster_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Update databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_kanban_id == "")
                {
                    MessageBox.Show("Error: Please select Kanban before update database!", "Database");
                }
                else
                {
                    dbUpdate("UPDATE test_model_master SET Kanban = '" + tbKBSearch.Text + "', ModelNo = '" + tbDBModel.Text + "', InnerA = '" + tbDBInnerA.Text + "', InnerB = '" + tbDBInnerB.Text + "', Carton = '" + tbDBCarton.Text + "', Export = '" + tbDBExport.Text + "', InnerMax = '" + tbDBInnerMax.Text + "', CartonMax = '" + tbDBCartonMax.Text + "' WHERE ID = '" + selected_kanban_id + "';");
                    dbSearchKanban();
                    selected_kanban_id = "";
                }
            }
        }
        //Delete kanban button
        private void btnDeleteMaster_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Delete databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_kanban_id == "")
                {
                    MessageBox.Show("Error: Please select Kanban before delete database!", "Database");
                }
                else
                {
                    dbDelete("DELETE FROM `test_model_master` WHERE ID = '" + selected_kanban_id + "';");
                    tbEditMasterClear();
                }
            }
        }
        //Listview click to select a item
        private void lvModelMaster_Click(object sender, EventArgs e)
        {
            var selected_item = lvModelMaster.SelectedItems[0];
            selected_kanban_id = selected_item.SubItems[0].Text;

            tbKBSearch.Text = selected_item.SubItems[1].Text;
            tbDBModel.Text = selected_item.SubItems[2].Text;
            tbDBInnerA.Text = selected_item.SubItems[3].Text;
            tbDBInnerB.Text = selected_item.SubItems[4].Text;
            tbDBCarton.Text = selected_item.SubItems[5].Text;
            tbDBExport.Text = selected_item.SubItems[6].Text;
            tbDBInnerMax.Text = selected_item.SubItems[7].Text;
            tbDBCartonMax.Text = selected_item.SubItems[8].Text;
        }
        private void lvAccount_Click(object sender, EventArgs e)
        {
            var selected_item = lvAccount.SelectedItems[0];
            selected_account_tagpass = selected_item.SubItems[0].Text;

            tbDBTagpass.Text = selected_item.SubItems[0].Text;
            tbDBOperatorID.Text = selected_item.SubItems[1].Text;
            tbDBName.Text = selected_item.SubItems[2].Text;
            tbDBSurname.Text = selected_item.SubItems[3].Text;
            tbDBPosition.Text = selected_item.SubItems[4].Text;
        }
        //Edit Account button
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if(tbDBTagpass.Text != "")
            {
                DialogResult dialog_result = MessageBox.Show("Are you sure to insert new Account?", "Database", MessageBoxButtons.YesNo);
                if (dialog_result == DialogResult.Yes)
                {
                    dbCreate("INSERT INTO test_account (Tagpass, OperatorID, Name, Surname, Position) VALUES('" + tbDBTagpass.Text + "', '" + tbDBOperatorID.Text + "', '" + tbDBName.Text + "', '" + tbDBSurname.Text + "', '" + tbDBPosition.Text + "');");
                    tbEditAccountClear();
                }
            }
        }
        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Update databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_account_tagpass == "")
                {
                    MessageBox.Show("Error: Please select Tagpass before update database!", "Database");
                }
                else
                {
                    dbUpdate("UPDATE test_account SET Tagpass = '" + tbDBTagpass.Text + "', OperatorID = '" + tbDBOperatorID.Text + "', Name = '" + tbDBName.Text + "', Surname = '" + tbDBSurname.Text + "', Position = '" + tbDBPosition.Text + "' WHERE Tagpass = '" + selected_account_tagpass + "';");
                    dbSearchTagpass();
                    selected_account_tagpass = "";
                }
            }
        }
        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Delete databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_account_tagpass == "")
                {
                    MessageBox.Show("Error: Please select Kanban before delete database!", "Database");
                }
                else
                {
                    dbDelete("DELETE FROM `test_account` WHERE Tagpass = '" + selected_account_tagpass + "';");
                    tbEditAccountClear();
                }
            }
        }
        //Account Tagpass textbox
        private void tbDBTagpass_TextChanged(object sender, EventArgs e)
        {
            if (tbDBTagpass.Text == "")
            {
                refreshListViewAccount();
                tbEditAccountClear();
            }
            else
            {
                lvAccount.Items.Clear();
                string TABLE = "test_account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE Tagpass LIKE '%" + tbDBTagpass.Text + "%' ORDER BY CAST(Tagpass AS UNSIGNED);";
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                        ListViewItem list = new ListViewItem(dr["Tagpass"].ToString());
                        list.SubItems.Add(dr["OperatorID"].ToString());
                        list.SubItems.Add(dr["Name"].ToString());
                        list.SubItems.Add(dr["Surname"].ToString());
                        list.SubItems.Add(dr["Position"].ToString());
                        lvAccount.Items.Add(list);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
            }
        }
        //K/B* Textbox text is changed(Search)
        private void tbKBSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbKBSearch.Text == "")
            {
                refreshListViewMaster();
                tbEditMasterClear();
            }
            else
            {
                //need to create function
                lvModelMaster.Items.Clear();
                //Database display Search data from table
                string TABLE = "test_model_master";
                string queryList = "SELECT * FROM " + TABLE + " WHERE Kanban LIKE '%" + tbKBSearch.Text + "%' ORDER BY CAST(Kanban AS UNSIGNED);";
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
                DataTable dt = new DataTable();
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    da.Fill(dt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                        ListViewItem list = new ListViewItem(dr["ID"].ToString());
                        list.SubItems.Add(dr["Kanban"].ToString());
                        list.SubItems.Add(dr["ModelNo"].ToString());
                        list.SubItems.Add(dr["InnerA"].ToString());
                        list.SubItems.Add(dr["InnerB"].ToString());
                        list.SubItems.Add(dr["Carton"].ToString());
                        list.SubItems.Add(dr["Export"].ToString());
                        list.SubItems.Add(dr["InnerMax"].ToString());
                        list.SubItems.Add(dr["CartonMax"].ToString());
                        lvModelMaster.Items.Add(list);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
            }
        }
        public void portsCloser()
        {
            try
            {
                if (port1 != null)
                {
                    port1.Close();
                    lIsPort1Open.Text = "Port1: Offine";
                    lIsPort1Open.BackColor = System.Drawing.Color.Crimson;
                }
                if (port2 != null)
                {
                    port2.Close();
                    lIsPort2Open.Text = "Port2: Offine";
                    lIsPort2Open.BackColor = System.Drawing.Color.Crimson;
                }
                if (port3 != null)
                {
                    port3.Close();
                    lIsPort3Open.Text = "Port3: Offine";
                    lIsPort3Open.BackColor = System.Drawing.Color.Crimson;
                }
                if (port4 != null)
                {
                    port4.Close();
                    lIsPort4Open.Text = "Port4: Offine";
                    lIsPort4Open.BackColor = System.Drawing.Color.Crimson;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void portsOpener()
        {
            try
            {
                if (cbPort1.Text != "")
                {
                    port1 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port1, WindowsForms_packing_line.Properties.Settings.Default.Baudrate1, WindowsForms_packing_line.Properties.Settings.Default.Paritybit1, WindowsForms_packing_line.Properties.Settings.Default.Datasize1, WindowsForms_packing_line.Properties.Settings.Default.Stopbits1);
                    port1.Open();
                    port1.DataReceived += new SerialDataReceivedEventHandler(dataReceiver1);
                    if (port1.IsOpen)
                    {
                        //set port1 Status: Online
                        lIsPort1Open.Text = "Port1: Online";
                        lIsPort1Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                if (cbPort2.Text != "")
                {
                    port2 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port2, WindowsForms_packing_line.Properties.Settings.Default.Baudrate2, WindowsForms_packing_line.Properties.Settings.Default.Paritybit2, WindowsForms_packing_line.Properties.Settings.Default.Datasize2, WindowsForms_packing_line.Properties.Settings.Default.Stopbits2);
                    port2.Open();
                    port2.DataReceived += new SerialDataReceivedEventHandler(dataReceiver2);
                    if (port2.IsOpen)
                    {
                        //set port1 Status: Online
                        lIsPort2Open.Text = "Port2: Online";
                        lIsPort2Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                if (cbPort3.Text != "")
                {
                    port3 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port3, WindowsForms_packing_line.Properties.Settings.Default.Baudrate3, WindowsForms_packing_line.Properties.Settings.Default.Paritybit3, WindowsForms_packing_line.Properties.Settings.Default.Datasize3, WindowsForms_packing_line.Properties.Settings.Default.Stopbits3);
                    port3.Open();
                    port3.DataReceived += new SerialDataReceivedEventHandler(dataReceiver3);
                    if (port3.IsOpen)
                    {
                        //set port1 Status: Online
                        lIsPort3Open.Text = "Port3: Online";
                        lIsPort3Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                if (cbPort4.Text != "")
                {
                    port4 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port4, WindowsForms_packing_line.Properties.Settings.Default.Baudrate4, WindowsForms_packing_line.Properties.Settings.Default.Paritybit4, WindowsForms_packing_line.Properties.Settings.Default.Datasize4, WindowsForms_packing_line.Properties.Settings.Default.Stopbits4);
                    port4.Open();
                    port4.DataReceived += new SerialDataReceivedEventHandler(dataReceiver4);
                    if (port4.IsOpen)
                    {
                        //set port1 Status: Online
                        lIsPort4Open.Text = "Port4: Online";
                        lIsPort4Open.BackColor = System.Drawing.Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void initialPorts()
        {
            cbPort1.Text = WindowsForms_packing_line.Properties.Settings.Default.Port1;
            cbBaudrate1.Text = WindowsForms_packing_line.Properties.Settings.Default.Baudrate1.ToString();
            cbParitybit1.Text = WindowsForms_packing_line.Properties.Settings.Default.Paritybit1.ToString();
            cbDatasize1.Text = WindowsForms_packing_line.Properties.Settings.Default.Datasize1.ToString();
            cbStopbits1.Text = WindowsForms_packing_line.Properties.Settings.Default.Stopbits1.ToString();

            cbPort2.Text = WindowsForms_packing_line.Properties.Settings.Default.Port2;
            cbBaudrate2.Text = WindowsForms_packing_line.Properties.Settings.Default.Baudrate2.ToString();
            cbParitybit2.Text = WindowsForms_packing_line.Properties.Settings.Default.Paritybit2.ToString();
            cbDatasize2.Text = WindowsForms_packing_line.Properties.Settings.Default.Datasize2.ToString();
            cbStopbits2.Text = WindowsForms_packing_line.Properties.Settings.Default.Stopbits2.ToString();
            
            cbPort3.Text = WindowsForms_packing_line.Properties.Settings.Default.Port3;
            cbBaudrate3.Text = WindowsForms_packing_line.Properties.Settings.Default.Baudrate3.ToString();
            cbParitybit3.Text = WindowsForms_packing_line.Properties.Settings.Default.Paritybit3.ToString();
            cbDatasize3.Text = WindowsForms_packing_line.Properties.Settings.Default.Datasize3.ToString();
            cbStopbits3.Text = WindowsForms_packing_line.Properties.Settings.Default.Stopbits3.ToString();

            cbPort4.Text = WindowsForms_packing_line.Properties.Settings.Default.Port4;
            cbBaudrate4.Text = WindowsForms_packing_line.Properties.Settings.Default.Baudrate4.ToString();
            cbParitybit4.Text = WindowsForms_packing_line.Properties.Settings.Default.Paritybit4.ToString();
            cbDatasize4.Text = WindowsForms_packing_line.Properties.Settings.Default.Datasize4.ToString();
            cbStopbits4.Text = WindowsForms_packing_line.Properties.Settings.Default.Stopbits4.ToString();
        }
        //SQL Connect, Get
        public void qeuryMax()
        {
            string queryList = "SELECT * FROM test_model_master WHERE Kanban = '" + tbKanban.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    innerbox_max = reader.GetInt32("InnerMax");     //Get InnerMax from db
                    cartonbox_max = reader.GetInt32("CartonMax");   //Get CartonMax from db
                    lNeedCarton.Text = "Need: " + carton_need.ToString();   //Display - (Carton)Need: 0
                    lNeedExport.Text = "Need: " + export_need.ToString();   //Display - (Export)Need: 0
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void queryInnerA()
        {
            string queryList = "SELECT * FROM test_model_master WHERE Kanban = '" + tbKanban.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = reader.GetString("InnerA");
                    lMasterA.Text = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void queryInnerB()
        {
            string queryList = "SELECT * FROM test_model_master WHERE Kanban = '" + tbKanban.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = reader.GetString("InnerB");
                    lMasterB.Text = WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void queryCarton()
        {
            string queryList = "SELECT * FROM test_model_master WHERE Kanban = '" + tbKanban.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = reader.GetString("Carton");
                    lMasterCarton.Text = WindowsForms_packing_line.Properties.Settings.Default.CartonMaster;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void queryExport()
        {
            string queryList = "SELECT * FROM test_model_master WHERE Kanban = '" + tbKanban.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                while (reader.Read())
                {
                    WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = reader.GetString("Export");
                    lMasterExport.Text = WindowsForms_packing_line.Properties.Settings.Default.ExportMaster;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void refreshListViewMaster()
        {
            lvModelMaster.Items.Clear();
            //Database display all data from table
            string TABLE = "test_model_master";
            string queryList = "SELECT * FROM " + TABLE + " ORDER BY CAST(Kanban AS UNSIGNED);";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
            DataTable dt = new DataTable();
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                    ListViewItem list = new ListViewItem(dr["ID"].ToString());
                    list.SubItems.Add(dr["Kanban"].ToString());
                    list.SubItems.Add(dr["ModelNo"].ToString());
                    list.SubItems.Add(dr["InnerA"].ToString());
                    list.SubItems.Add(dr["InnerB"].ToString());
                    list.SubItems.Add(dr["Carton"].ToString());
                    list.SubItems.Add(dr["Export"].ToString());
                    list.SubItems.Add(dr["InnerMax"].ToString());
                    list.SubItems.Add(dr["CartonMax"].ToString());
                    lvModelMaster.Items.Add(list);
                }
                lvModelMaster.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvModelMaster.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void dbSearchKanban()
        {
            lvModelMaster.Items.Clear();
            string TABLE = "test_model_master";
            string queryList = "SELECT * FROM " + TABLE + " WHERE Kanban = '" + tbKBSearch.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
            DataTable dt = new DataTable();
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                    ListViewItem list = new ListViewItem(dr["ID"].ToString());
                    list.SubItems.Add(dr["Kanban"].ToString());
                    list.SubItems.Add(dr["ModelNo"].ToString());
                    list.SubItems.Add(dr["InnerA"].ToString());
                    list.SubItems.Add(dr["InnerB"].ToString());
                    list.SubItems.Add(dr["Carton"].ToString());
                    list.SubItems.Add(dr["Export"].ToString());
                    list.SubItems.Add(dr["InnerMax"].ToString());
                    list.SubItems.Add(dr["CartonMax"].ToString());
                    lvModelMaster.Items.Add(list);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void refreshListViewAccount()
        {
            lvAccount.Items.Clear();
            string TABLE = "test_account";
            string queryList = "SELECT * FROM " + TABLE + " ORDER BY CAST(Tagpass AS UNSIGNED);";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
            DataTable dt = new DataTable();
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                    ListViewItem list = new ListViewItem(dr["Tagpass"].ToString());
                    list.SubItems.Add(dr["OperatorID"].ToString());
                    list.SubItems.Add(dr["Name"].ToString());
                    list.SubItems.Add(dr["Surname"].ToString());
                    list.SubItems.Add(dr["Position"].ToString());
                    lvAccount.Items.Add(list);
                }
                lvAccount.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvAccount.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void dbSearchTagpass()
        {
            lvAccount.Items.Clear();
            string TABLE = "test_account";
            string queryList = "SELECT * FROM " + TABLE + " WHERE Tagpass = '" + tbDBTagpass.Text + "';";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
            DataTable dt = new DataTable();
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                da.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];    //dr["Column Name from db"]
                    ListViewItem list = new ListViewItem(dr["Tagpass"].ToString());
                    list.SubItems.Add(dr["OperatorID"].ToString());
                    list.SubItems.Add(dr["Name"].ToString());
                    list.SubItems.Add(dr["Surname"].ToString());
                    list.SubItems.Add(dr["Position"].ToString());
                    lvAccount.Items.Add(list);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        //SQL connect, Edit
        public void dbCreate(string qeury_s)
        {
            string queryList = qeury_s;
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                MessageBox.Show("Insert Success", "Dababase");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        public void dbUpdate(string query_s)
        {
            string queryList = query_s;
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                MessageBox.Show("Update Success!", "Database");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }

        

        public void dbDelete(string query_s)
        {
            string queryList = query_s;
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                MessageBox.Show("Delete Success!", "Database");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }
        //Clear Textbox Edit Database page
        public void tbEditMasterClear()
        {
            tbKBSearch.Clear();
            tbDBModel.Clear();
            tbDBInnerA.Clear();
            tbDBInnerB.Clear();
            tbDBCarton.Clear();
            tbDBExport.Clear();
            tbDBInnerMax.Clear();
            tbDBCartonMax.Clear();
        }
        public void tbEditAccountClear()
        {
            tbDBTagpass.Clear();
            tbDBOperatorID.Clear();
            tbDBName.Clear();
            tbDBSurname.Clear();
            tbDBPosition.Clear();
        }
        public void roleChecker(string s)
        {
            //Admin

            if (s== "Operator")
            {
                enableTab(Settings, false);
                tbDBModel.Enabled = false;
                tbDBInnerA.Enabled = false;
                tbDBInnerB.Enabled = false;
                tbDBCarton.Enabled = false;
                tbDBExport.Enabled = false;
                tbDBInnerMax.Enabled = false;
                tbDBCartonMax.Enabled = false;
                btnCreateMaster.Hide();
                btnUpdateMaster.Hide();
                btnDeleteMaster.Hide();
                hideTab(Account, true);
            }
            else if (s=="Supervisor")
            {
                tbDBOperatorID.Enabled = false;
                tbDBName.Enabled = false;
                tbDBSurname.Enabled = false;
                tbDBPosition.Enabled = false;
                btnCreateAccount.Hide();
                btnUpdateAccount.Hide();
                btnDeleteAccount.Hide();
            }
        }
        public static void enableTab(TabPage page, bool enable)
        {
            foreach (Control ctl in page.Controls) ctl.Enabled = enable;
        }
        public static void hideTab(TabPage page, bool hide)
        {
            if (hide) //Hide = true
            {
                foreach (Control ctl in page.Controls) ctl.Hide();
            }
            else
            {
                foreach (Control ctl in page.Controls) ctl.Show();
            }
        }
    }
}