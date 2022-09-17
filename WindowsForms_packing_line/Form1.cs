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
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace WindowsForms_packing_line
{
    public partial class Form1 : Form
    {
        private SerialPort port1;
        private SerialPort port2;
        private SerialPort port3;
        private SerialPort port4;
        public static SerialPort portRFID = new SerialPort("COM7", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        public static string connectStr = "server=" + WindowsForms_packing_line.Properties.Settings.Default.dbIPServer + ";port=3306;Database=packing_line_element;uid=root;pwd=;SslMode=none;";
        private int qty_kanban, qty_current, innerbox_max, cartonbox_max;   //get from db
        int total = 0, inner_count = 0, carton_count = 0 , carton_need = 0, export_need = 0; //counter +1 the larger box
        string inner_a_master = "";  //inner a master
        string inner_b_master = "";  //inner b master
        string carton_master = "";   //carton master
        string export_master = "";   //export master
        string selected_kanban_id = ""; //temp_str kanban for update database
        string selected_account_tagpass = ""; //temp_str account for update database
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.None;
            refreshListViewMaster();
            refreshListViewAccount();
            initialPorts();
            this.ActiveControl = tbLogin;
            btnLogout.Hide();
            WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = "";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Ports Indicator(in ComboBoxes)
            string[] getPorts = SerialPort.GetPortNames();
            cbPort1.Items.AddRange(getPorts);
            cbPort2.Items.AddRange(getPorts);
            cbPort3.Items.AddRange(getPorts);
            cbPort4.Items.AddRange(getPorts);
            cbPort1.Text = WindowsForms_packing_line.Properties.Settings.Default.Port1;
            cbPort2.Text = WindowsForms_packing_line.Properties.Settings.Default.Port2;
            cbPort3.Text = WindowsForms_packing_line.Properties.Settings.Default.Port3;
            cbPort4.Text = WindowsForms_packing_line.Properties.Settings.Default.Port4;
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
                        btnLogout.Show();
                        roleChecker(reader.GetString("Position"));
                        tbKanban.Focus();
                        Invoke((MethodInvoker)delegate { tbLogin.Enabled = false; });
                        portsCloser();
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
        }//OK
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
                    btnLogout.Show();
                    roleChecker(reader.GetString("Position"));
                    tbKanban.Focus();
                    Invoke((MethodInvoker)delegate { tbLogin.Enabled = false; });
                    portsCloser();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
                tbLogin.Focus();
            }
        }//OK
        private void btnRFID_Click(object sender, EventArgs e)
        {
            try
            {
                portRFID.Open();
                portRFID.DataReceived += new SerialDataReceivedEventHandler(dataReceiverRFID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            tbLogin.Focus();
        }//Waittest
        private void btnLogout_Click(object sender, EventArgs e)
        {
            lOperatorID.Text = "Operator ID: ";
            lOperatorName.Text = "Operator Name: ";
            lPosition.Text = "Position: ";
            pLogin.Show();
            tbLogin.Clear();
            Invoke((MethodInvoker)delegate { tbLogin.Enabled = true; });
            Invoke((MethodInvoker)delegate { btnLogout.Hide(); });
            tbLogin.Focus();
        }//OK
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
                    tbQTY.Text = kanban_array[5];
                    tbQTY.Focus();
                    qty_kanban = int.Parse(kanban_array[5]);
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
                isInteger = int.TryParse(tbQTY.Text, out qty_current);
                if (!isInteger)
                {
                    MessageBox.Show("Error: K/B is empty or QTY is incorrect!", "Checker");
                }
                else if (tbModel.Text == "")
                {
                    MessageBox.Show("Error: Please comfirm K/B and Model!", "Checker");
                }
                else
                {
                    qeuryMax();
                    queryInnerA();
                    queryInnerB();
                    queryCarton();
                    queryExport();
                    lTotal.Text = "Total: " + qty_current.ToString();
                    total = 0;
                    inner_count = 0;
                    carton_count = 0;
                    carton_need = 0;
                    export_need = 0;
                    Invoke((MethodInvoker)delegate { btnStart.Hide(); });
                    tbKanban.ReadOnly = true;
                    if (typeCheck() == 3)
                    {
                        if (qty_current <= qty_kanban)
                        {
                            export_need++;
                            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                        }
                        else
                        {
                            if (qty_current % qty_kanban == 0)
                            {
                                export_need = qty_current / qty_kanban;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                            }
                            else
                            {
                                export_need = qty_current / qty_kanban;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                                if ((qty_current % qty_kanban) < qty_kanban)
                                {
                                    export_need++;
                                    Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                                }
                            }
                        }                        
                    }
                    else if (typeCheck() == 4)
                    {
                        if (qty_current % innerbox_max == 0)
                        {
                            carton_need = qty_current / innerbox_max;
                            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                        }
                        else
                        {
                            carton_need = qty_current / innerbox_max;
                            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                            if ((qty_current % innerbox_max) < innerbox_max)
                            {
                                carton_need++;
                                Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }//OK
        //Data Receiver
        private void dataReceiverRFID(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(100);
                int input_value = portRFID.ReadByte();
                byte[] buffer = new byte[input_value];
                portRFID.Read(buffer, 0, input_value);
                string s_buffer = "";
                for (int i = 0; i < input_value; i++)
                {
                    s_buffer += s_buffer[i];
                }
                MessageBox.Show(s_buffer);
                if(tbLogin.Enabled == true)
                {
                    Invoke((MethodInvoker)delegate { tbLogin.Text = s_buffer; });
                }
                else if (tbDBTagpass.Enabled == true)
                {
                    Invoke((MethodInvoker)delegate { tbDBTagpass.Text = s_buffer; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Waittest
        private void dataReceiver1(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(60);
                string input_value = port1.ReadExisting();
                Invoke((MethodInvoker)delegate { tbInnerBoxA.Text = input_value; });
                inner_a_master = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                if (input_value.Equals(inner_a_master))
                {
                    if(qty_current - inner_count > 0)
                    {
                        //Log
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                        
                        inner_count++;
                        total = qty_current - inner_count;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (total).ToString(); });

                        if (typeCheck() == 1)
                        {
                            if (inner_count % innerbox_max == 0)
                            {
                                carton_need = inner_count / innerbox_max;
                                Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                            }
                            else if (qty_current - inner_count == 0)//Fraction
                            {
                                carton_need++;
                                Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                            }
                        }
                        else if (typeCheck() == 2)
                        {
                            if (inner_count % cartonbox_max == 0)
                            {
                                export_need = inner_count / cartonbox_max;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                            }
                            else if (qty_current - inner_count == 0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                            }
                        }
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add("did not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    }
                }
                else
                {
                    portsCloser();
                    alarmAuth();
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add("The alarm has been reset."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiver2(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(60);
                string input_value = port2.ReadExisting();
                Invoke((MethodInvoker)delegate { tbInnerBoxB.Text = input_value; });
                inner_b_master = WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster;
                if (input_value.Equals(inner_b_master))
                {
                    if (qty_current - inner_count > 0)
                    {
                        //Log
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                        
                        inner_count++;
                        total = qty_current - inner_count;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (total).ToString(); });

                        if (typeCheck() == 1)
                        {
                            if (inner_count % innerbox_max == 0)
                            {
                                carton_need = inner_count / innerbox_max;
                                Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                            }
                            else if (qty_current - inner_count == 0)//Fraction
                            {
                                carton_need++;
                                Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                            }
                        }
                        else if (typeCheck() == 2)
                        {
                            if (inner_count % cartonbox_max == 0)
                            {
                                export_need = inner_count / cartonbox_max;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                            }
                            else if (qty_current - inner_count == 0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                            }
                        }
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add("did not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    }
                }
                else
                {
                    portsCloser();
                    alarmAuth();
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add("The alarm has been reset."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiver3(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(60);
                string input_value = port3.ReadExisting();
                Invoke((MethodInvoker)delegate { tbCartonBox.Text = input_value; });
                carton_master = WindowsForms_packing_line.Properties.Settings.Default.CartonMaster;
                if (input_value.Equals(carton_master))
                {
                    if (carton_need > 0)
                    {
                        //Log
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tCarton Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                        
                        carton_need--;
                        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need; });

                        if (typeCheck() == 1)
                        {
                            carton_count++;
                            if (carton_count % cartonbox_max == 0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                            }
                            else if (qty_current - inner_count == 0 && carton_need == 0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                            }
                        }
                        else if (typeCheck() == 4)
                        {
                            inner_count += innerbox_max;
                            total = qty_current - inner_count;
                            if (total < 0) { total = 0; }
                            Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (total).ToString(); });

                            carton_count++;
                            if (carton_count % cartonbox_max == 0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                            }
                            else if (total == 0 && carton_need ==0)
                            {
                                export_need++;
                                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                            }
                        }

                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add("did not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    }
                }
                else
                {
                    portsCloser();
                    alarmAuth();
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tCarton Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add("The alarm has been reset."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Cannot decrease
        private void dataReceiver4(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(60);
                string input_value = port4.ReadExisting();
                Invoke((MethodInvoker)delegate { tbExportBox.Text = input_value; });
                export_master = WindowsForms_packing_line.Properties.Settings.Default.ExportMaster;
                if (input_value.Equals(export_master))
                {
                    if (export_need > 0) //type1-2
                    {
                        //Log
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });

                        export_need--;
                        Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });

                        if (typeCheck() == 3)
                        {
                            inner_count += cartonbox_max;
                            total = qty_current - inner_count;
                            if (total < 0) { total = 0; }
                            Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (total).ToString(); });
                        }
                    }
                    else if (export_need == 0 && total == 0 && inner_count != 0)
                    {
                        portsCloser();
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add("Need to reset Kanban."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                        MessageBox.Show("Alarm reset this Kanban! "+ total);
                        inner_count = 0;
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate { lbLog.Items.Add("did not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    }
                }
                else
                {
                    portsCloser();
                    alarmAuth();
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    Invoke((MethodInvoker)delegate { lbLog.Items.Add("The alarm has been reset."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Cannot decrease
        //Decrease a counter -1 at a time
        private void btnDecreaseInnerA_Click(object sender, EventArgs e)
        {
            if (inner_count > 0)
            {
                inner_count--;
                Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty_current - inner_count).ToString(); });
                Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                if (typeCheck() == 1)
                {
                    carton_need = inner_count / innerbox_max;
                    Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                }
                else if (typeCheck() ==2)
                {
                    export_need = inner_count / cartonbox_max;
                    Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                }
            }
        }
        private void btnDecreaseInnerB_Click(object sender, EventArgs e)
        {
            if (inner_count > 0)
            {
                inner_count--;
                Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty_current - inner_count).ToString(); });
                Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                if (typeCheck() == 1)
                {
                    carton_need = inner_count / innerbox_max;
                    Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString(); });
                }
                else if (typeCheck() == 2)
                {
                    export_need = inner_count / cartonbox_max;
                    Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString(); });
                }
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
                        lIsPort1Open.Text = "Port1: Offline";
                        lIsPort1Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port1.IsOpen == false)
                    {
                        port1.Open();
                        lIsPort1Open.Text = "Port1: Online ";
                        lIsPort1Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else if(cbPort1.Text != "")
                {
                    portsOpener();
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
                        lIsPort2Open.Text = "Port2: Offline";
                        lIsPort2Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port2.IsOpen == false)
                    {
                        port2.Open();
                        lIsPort2Open.Text = "Port2: Online ";
                        lIsPort2Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else if (cbPort2.Text != "")
                {
                    portsOpener();
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
                        lIsPort3Open.Text = "Port3: Offline";
                        lIsPort3Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port3.IsOpen == false)
                    {
                        port3.Open();
                        lIsPort3Open.Text = "Port3: Online ";
                        lIsPort3Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else if (cbPort3.Text != "")
                {
                    portsOpener();
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
                        lIsPort4Open.Text = "Port4: Offline";
                        lIsPort4Open.BackColor = System.Drawing.Color.Crimson;
                    }
                    else if (port4.IsOpen == false)
                    {
                        port4.Open();
                        lIsPort4Open.Text = "Port4: Online ";
                        lIsPort4Open.BackColor = System.Drawing.Color.Green;
                    }
                }
                else if (cbPort4.Text != "")
                {
                    portsOpener();
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
        private void btnResetKB_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Are you sure to reset Kanban?", "Reset Kanban", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                alarmAuth();
                resetChecker();
                tbKanban.ReadOnly = false;
                tbKanban.Focus();
            }
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
        private void btnMasterClear_Click(object sender, EventArgs e)
        {
            tbEditMasterClear();
        }//OK
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
        }//OK
        private void lvAccount_Click(object sender, EventArgs e)
        {
            var selected_item = lvAccount.SelectedItems[0];
            selected_account_tagpass = selected_item.SubItems[0].Text;

            tbDBTagpass.Text = selected_item.SubItems[0].Text;
            tbDBOperatorID.Text = selected_item.SubItems[1].Text;
            tbDBName.Text = selected_item.SubItems[2].Text;
            tbDBSurname.Text = selected_item.SubItems[3].Text;
            cbDBPosition.Text = selected_item.SubItems[4].Text;
        }//OK
        //Edit Account button
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if(tbDBTagpass.Text != "")
            {
                DialogResult dialog_result = MessageBox.Show("Are you sure to insert new Account?", "Database", MessageBoxButtons.YesNo);
                if (dialog_result == DialogResult.Yes)
                {
                    dbCreate("INSERT INTO test_account (Tagpass, OperatorID, Name, Surname, Position) VALUES('" + tbDBTagpass.Text + "', '" + tbDBOperatorID.Text + "', '" + tbDBName.Text + "', '" + tbDBSurname.Text + "', '" + cbDBPosition.Text + "');");
                    tbEditAccountClear();
                }
            }
        }//OK
        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Update databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_account_tagpass == "")
                {
                    MessageBox.Show("Error: Please select Tagpass before update database!", "Database");
                }
                else if(tbDBOperatorID.Text != "" && tbDBName.Text != "" && tbDBSurname.Text != "" && cbDBPosition.Text != "")
                {
                    dbUpdate("UPDATE test_account SET Tagpass = '" + tbDBTagpass.Text + "', OperatorID = '" + tbDBOperatorID.Text + "', Name = '" + tbDBName.Text + "', Surname = '" + tbDBSurname.Text + "', Position = '" + cbDBPosition.Text + "' WHERE Tagpass = '" + selected_account_tagpass + "';");
                    dbSearchTagpass();
                    selected_account_tagpass = "";
                }
            }
        }//OK
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
        }//OK
        private void btnAccountClear_Click(object sender, EventArgs e)
        {
            tbEditAccountClear();
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
        }//OK
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
        }//OK
        public void portsCloser()
        {
            try
            {
                if (port1 != null)
                {
                    port1.Close();
                    Invoke((MethodInvoker)delegate {
                        lIsPort1Open.Text = "Port1: Offline";
                        lIsPort1Open.BackColor = System.Drawing.Color.Crimson;
                    });
                }
                if (port2 != null)
                {
                    port2.Close();
                    Invoke((MethodInvoker)delegate
                    {
                        lIsPort2Open.Text = "Port2: Offline";
                        lIsPort2Open.BackColor = System.Drawing.Color.Crimson;
                    });
                }
                if (port3 != null)
                {
                    port3.Close();
                    Invoke((MethodInvoker)delegate
                    {
                        lIsPort3Open.Text = "Port3: Offline";
                        lIsPort3Open.BackColor = System.Drawing.Color.Crimson;
                    });
                }
                if (port4 != null)
                {
                    port4.Close();
                    Invoke((MethodInvoker)delegate
                    {
                        lIsPort4Open.Text = "Port4: Offline";
                        lIsPort4Open.BackColor = System.Drawing.Color.Crimson;
                    });
                }
                if (portRFID != null)
                {
                    portRFID.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
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
                        lIsPort1Open.Text = "Port1: Online ";
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
                        lIsPort2Open.Text = "Port2: Online ";
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
                        lIsPort3Open.Text = "Port3: Online ";
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
                        lIsPort4Open.Text = "Port4: Online ";
                        lIsPort4Open.BackColor = System.Drawing.Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
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
        }//Wait set initial ports
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
                    lNeedCarton.Text = "Scan: " + carton_need.ToString();   //Display - (Carton)Scan: 0
                    lNeedExport.Text = "Scan: " + export_need.ToString();   //Display - (Export)Scan: 0
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK
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
        }//OK

        

        public void tbEditAccountClear()
        {
            tbDBTagpass.Clear();
            tbDBOperatorID.Clear();
            tbDBName.Clear();
            tbDBSurname.Clear();
            cbDBPosition.SelectedIndex = -1;
        }//OK
        public void roleChecker(string s)
        {
            enableTab(Checker, true);
            enableTab(Settings, true);
            enableTab(Edit, true);
            enableTab(Account, true);
            hideTab(Settings, false);
            hideTab(Edit, false);
            hideTab(Account, false);

            tbDBModel.Enabled = true;
            tbDBInnerA.Enabled = true;
            tbDBInnerB.Enabled = true;
            tbDBCarton.Enabled = true;
            tbDBExport.Enabled = true;
            tbDBInnerMax.Enabled = true;
            tbDBCartonMax.Enabled = true;
            btnCreateMaster.Show();
            btnUpdateMaster.Show();
            btnDeleteMaster.Show();

            tbDBOperatorID.Enabled = true;
            tbDBName.Enabled = true;
            tbDBSurname.Enabled = true;
            cbDBPosition.Enabled = true;
            btnCreateAccount.Show();
            btnUpdateAccount.Show();
            btnDeleteAccount.Show();
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
                cbDBPosition.Enabled = false;
                btnCreateAccount.Hide();
                btnUpdateAccount.Hide();
                btnDeleteAccount.Hide();
            }
            else if (s=="Administrator")
            {

            }
            else
            {
                enableTab(Checker, false);
                hideTab(Settings, true);
                hideTab(Edit, true);
                hideTab(Account, true);
            }
        }//Waittest
        public static void enableTab(TabPage page, bool enable)
        {
            foreach (Control ctl in page.Controls) ctl.Enabled = enable;
        }//OK
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
        }//OK
        //Login
        //Alarm auth
        public void alarmAuth()
        {
            Form frm_overlay = new Form();
            try
            {
                using (Authentication auth = new Authentication())
                {
                    frm_overlay.StartPosition = FormStartPosition.Manual;
                    frm_overlay.FormBorderStyle = FormBorderStyle.None;
                    frm_overlay.Opacity = 0.70d;
                    frm_overlay.BackColor = Color.Black;
                    frm_overlay.WindowState = FormWindowState.Maximized;
                    frm_overlay.TopMost = false;
                    frm_overlay.Location = this.Location;
                    frm_overlay.ShowInTaskbar = false;
                    frm_overlay.Show();
                    auth.Owner = frm_overlay;
                    auth.ShowDialog();
                    frm_overlay.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                frm_overlay.Dispose();
            }
        }//Waittest
        //Reset Checker
        public void resetChecker()
        {
            WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = "";
            qty_current = 0;
            innerbox_max = 0;
            cartonbox_max = 0;
            inner_count = 0;
            carton_count = 0;
            carton_need = 0;
            export_need = 0;
            inner_a_master = "";
            inner_b_master = "";
            carton_master = "";
            export_master = "";
            tbKanban.Clear();
            tbModel.Clear();
            tbQTY.Clear();
            lMasterA.Text = "Master: -" + inner_a_master;
            lMasterB.Text = "Master: -" + inner_b_master;
            lMasterCarton.Text = "Master: -" + carton_master;
            lMasterExport.Text = "Master: -" + export_master;
            lNeedCarton.Text = "Scan: " + carton_need.ToString();
            lNeedExport.Text = "Scan: " + export_need.ToString();
            tbInnerBoxA.Clear();
            tbInnerBoxB.Clear();
            tbCartonBox.Clear();
            tbExportBox.Clear();
            lTotal.Text = "Total: " + qty_current.ToString();
            lbLog.Items.Clear();
            btnStart.Show();
        }//OK
        public byte typeCheck()
        {
            byte type = 0;
            if (lMasterA.Text != "" && lMasterB.Text != "" && lMasterCarton.Text != "")
            {
                type = 1;
            }
            else if (lMasterA.Text != "" && lMasterB.Text != "" && lMasterCarton.Text == "")
            {
                type = 2;
            }
            else if (lMasterA.Text == "" && lMasterB.Text == "" && lMasterCarton.Text == "")
            {
                type = 3;
            }
            else if (lMasterA.Text == "" && lMasterB.Text == "" && lMasterCarton.Text != "")
            {
                type = 4;
            }
            return type;
        }//Waittest
    }
}