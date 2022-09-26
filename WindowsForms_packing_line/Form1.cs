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
using EasyModbus;
using System.Runtime.CompilerServices;
using System.Security;
using System.Media;
using QiHe.CodeLib;

namespace WindowsForms_packing_line
{
    public partial class Form1 : Form
    {
        private SerialPort port1;
        private SerialPort port2;
        private SerialPort port3;
        private SerialPort port4;
        private SerialPort portTowerLamp, portPDFLink;
        public static SerialPort portRFID = new SerialPort("COM7", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
        public static string connectStr = "server=" + WindowsForms_packing_line.Properties.Settings.Default.dbIPServer + ";port=3306;Database=scannerstb;uid=root;pwd=;SslMode=none;";
        private SoundPlayer _sound_player1 = new SoundPlayer(WindowsForms_packing_line.Properties.Resources.ScannerOK);
        private SoundPlayer _sound_player2 = new SoundPlayer(WindowsForms_packing_line.Properties.Resources.ScannerOK2);
        private SoundPlayer _sound_player3 = new SoundPlayer(WindowsForms_packing_line.Properties.Resources.NotifyOK);
        private SoundPlayer _sound_player4 = new SoundPlayer(WindowsForms_packing_line.Properties.Resources.AlarmNG);
        //private ModbusClient modbus_tcp = new ModbusClient("192.168.1.110", 502);
        private int qty_kanban, qty_current, innerbox_max, cartonbox_max;   //get from db
        //int inner_count = 0, carton_count = 0, carton_need = 0, export_need = 0; //counter +1 the larger box
        //int carton_scanned = 0, export_scanned = 0; // carton box, export box when scanned
        string inner_a_master = "";  //inner a master
        string inner_b_master = "";  //inner b master
        string carton_master = "";   //carton master
        string export_master = "";   //export master
        string selected_kanban_name = ""; //temp_str kanban for update database
        string selected_account_tagpass = ""; //temp_str account for update database
        string kanban_master = "";  //kanban no for update actual table
        //NEW VARIABLE
        int qty = 0;
        int innerbox_counter = 0;
        int cartonbox_counter = 0;
        int exportbox_counter = 0;
        int total = 0;
        bool switchIsOn = false; //Start
        bool pauseIsOn = false; //Pause
        bool port3IsOn = false;
        bool port4IsOn = false;
        byte portAlarm = 0; //Port alarm 1-4
        public enum colors { red, yellow, green, buzzer, reset }
        //public List<ActualTable> actual_table { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            refreshListViewMaster();
            refreshListViewAccount();
            refreshDGVAcutalTable();
            showSettingPorts();
            portTLOpen();
            portPDFLinkOpen();
            this.ActiveControl = tbLogin;
            btnLogout.Hide();
            //CLEAR MASTERS
            WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = "";
            //actual_table = getActualTable();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //PORTS INDICATOR
            string[] getPorts = SerialPort.GetPortNames();
            cbPort1.Items.AddRange(getPorts);
            cbPort2.Items.AddRange(getPorts);
            cbPort3.Items.AddRange(getPorts);
            cbPort4.Items.AddRange(getPorts);
            cbPortTL.Items.AddRange(getPorts);
            cbPortLink.Items.AddRange(getPorts);
            //PORTS SAVED
            cbPort1.Text = WindowsForms_packing_line.Properties.Settings.Default.Port1;
            cbPort2.Text = WindowsForms_packing_line.Properties.Settings.Default.Port2;
            cbPort3.Text = WindowsForms_packing_line.Properties.Settings.Default.Port3;
            cbPort4.Text = WindowsForms_packing_line.Properties.Settings.Default.Port4;
            cbPortTL.Text = WindowsForms_packing_line.Properties.Settings.Default.PortTL;
            cbPortLink.Text = WindowsForms_packing_line.Properties.Settings.Default.PortLink;
            //STATUS
            greenAlarm();
        }
        //Login
        private void tbLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
            }
        }//OK
        private void btnLogin_Click(object sender, EventArgs e)
        {
            //string TABLE = "account";
            //string queryList = "SELECT * FROM " + TABLE + " WHERE tagpass = '" + tbLogin.Text + "';";
            //MySqlConnection dbconnect = new MySqlConnection(connectStr);
            //MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            //MySqlDataReader reader;
            //dbcommand.CommandTimeout = 100;
            //try
            //{
            //    dbconnect.Open();
            //    reader = dbcommand.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        lOperatorID.Text = "Operator ID: " + reader.GetString("operatorID");
            //        lOperatorName.Text = "Operator Name: " + reader.GetString("fname") + " " + reader.GetString("lname");
            //        lPosition.Text = "Position: " + reader.GetString("position");
            //        pLogin.Hide();
            //        btnLogout.Show();
            //        roleChecker(reader.GetString("position"));
            //        tbKanban.Focus();
            //        portsCloser();
            //    }
            //    tbLogin.SelectAll();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //finally
            //{
            //    dbconnect.Close();
            //    tbLogin.Focus();
            //}
            Login();
        }//OK
        private void btnRFID_Click(object sender, EventArgs e)
        {
            try
            {
                portRFID.Open();
                portRFID.DataReceived += new SerialDataReceivedEventHandler(dataReceiverRFIDLogin);
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
            Invoke((MethodInvoker)delegate { btnLogout.Hide(); });
            tbLogin.Focus();
        }//OK
        private void pbIcon_Click(object sender, EventArgs e)
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
            else if (cbPort1.Text == cbPortTL.Text)
            {
                cbPortTL.SelectedIndex = -1;
            }
            else if (cbPort1.Text == cbPortLink.Text)
            {
                cbPortLink.SelectedIndex = -1;
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
            else if (cbPort2.Text == cbPortTL.Text)
            {
                cbPortTL.SelectedIndex = -1;
            }
            else if (cbPort2.Text == cbPortLink.Text)
            {
                cbPortLink.SelectedIndex = -1;
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
            else if (cbPort3.Text == cbPortTL.Text)
            {
                cbPortTL.SelectedIndex = -1;
            }
            else if (cbPort3.Text == cbPortLink.Text)
            {
                cbPortLink.SelectedIndex = -1;
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
            else if (cbPort4.Text == cbPortTL.Text)
            {
                cbPortTL.SelectedIndex = -1;
            }
            else if (cbPort4.Text == cbPortLink.Text)
            {
                cbPortLink.SelectedIndex = -1;
            }
        }//OK
        private void cbPortTL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPortTL.Text == cbPort1.Text)
            {
                cbPort1.SelectedIndex = -1;
            }
            else if (cbPortTL.Text == cbPort2.Text)
            {
                cbPort2.SelectedIndex = -1;
            }
            else if (cbPortTL.Text == cbPort3.Text)
            {
                cbPort3.SelectedIndex = -1;
            }
            else if (cbPortTL.Text == cbPort4.Text)
            {
                cbPort4.SelectedIndex = -1;
            }
            else if (cbPortTL.Text == cbPortLink.Text)
            {
                cbPortLink.SelectedIndex = -1;
            }
        }//Waittest
        private void cbPortLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPortLink.Text == cbPort1.Text)
            {
                cbPort1.SelectedIndex = -1;
            }
            else if (cbPortLink.Text == cbPort2.Text)
            {
                cbPort2.SelectedIndex = -1;
            }
            else if (cbPortLink.Text == cbPort3.Text)
            {
                cbPort3.SelectedIndex = -1;
            }
            else if (cbPortLink.Text == cbPort4.Text)
            {
                cbPort4.SelectedIndex = -1;
            }
            else if (cbPortLink.Text == cbPortTL.Text)
            {
                cbPortTL.SelectedIndex = -1;
            }
        }//Waittest
        //Set Baud rate
        private void cbBaudrate1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbBaudrate1.Text != "")
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
            if (cbStopbits1.Text != "")
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
            WindowsForms_packing_line.Properties.Settings.Default.PortTL = "";
            WindowsForms_packing_line.Properties.Settings.Default.PortLink = "";
            WindowsForms_packing_line.Properties.Settings.Default.Port1 = cbPort1.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port2 = cbPort2.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port3 = cbPort3.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Port4 = cbPort4.Text;
            WindowsForms_packing_line.Properties.Settings.Default.PortTL = cbPortTL.Text;
            WindowsForms_packing_line.Properties.Settings.Default.PortLink = cbPortLink.Text;
            WindowsForms_packing_line.Properties.Settings.Default.Save();
            //portsCloser();
            //portsOtherClose();
            //portsOpener();
            //portsOtherOpen();
        }//OK
        //Data Receiver and Output
        private void tbKanban_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string quantity = "0";
                if(checkVerticalBar(tbKanban.Text) == true)
                {
                    string[] kanban_array = tbKanban.Text.Split('|');
                    Invoke((MethodInvoker)delegate { tbKanban.Text = kanban_array[2]; });
                    quantity = kanban_array[5];
                }
                string TABLE = "modelmaster";
                string queryList = "SELECT * FROM " + TABLE + " WHERE kanban = '" + tbKanban.Text + "';";
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
                        kanban_master = tbKanban.Text;
                        tbModel.Text = reader.GetString("modelName"); //Get Model
                        tbQTY.Text = quantity;
                        //tbQTY.Focus();
                        //qty_kanban = int.Parse(kanban_array[5]);
                        qty = int.Parse(quantity);
                        Calculate();
                    }
                    tbKanban.SelectAll();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                    //MessageBox.Show(kanban_master, "SendToUSB");//Send to USB
                    sendToRS232(kanban_master);
                }
            }
        }//Waitfix
        //Start Button
        private void btnStart_Click(object sender, EventArgs e)
        {
            bool isInteger = false;
            try
            {
                isInteger = int.TryParse(tbQTY.Text, out qty);
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
                    if (switchIsOn == false)
                    {
                        //Be starting
                        btnStart.BackColor = Color.Crimson;
                        btnStart.Text = "STOP";
                        switchIsOn = true;
                        //MessageBox.Show(qty.ToString());
                        portsCloser();
                        if (typeCheck() == 1)
                        {
                            port1Open();
                            port2Open();
                        }
                        else if (typeCheck() == 2)
                        {
                            port1Open();
                            port2Open();
                        }
                        else if (typeCheck() == 3)
                        {
                            total = qty;
                            Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                            Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + innerbox_max + " / " + innerbox_max; }); //cartonbox_max = eb max, innerbox_max = ob max
                            if (total < innerbox_max)
                            {
                                if (total < 0) { total = 0; }
                                Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + total + " / " + innerbox_max; }); //cartonbox_max = eb max, innerbox_max = ob max
                            }
                            Invoke((MethodInvoker)delegate { pArrow2.Visible = true; });
                            port4Open();
                        }
                        else if (typeCheck() == 4)
                        {
                            total = qty;
                            Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                            Invoke((MethodInvoker)delegate { pArrow1.Visible = true; });
                            port3Open();
                        }
                    }
                    else
                    {
                        //Be stoping
                        btnStart.BackColor = Color.MediumAquamarine;
                        btnStart.Text = "START";
                        switchIsOn = false;
                        portsCloser();
                        resetDefault();
                    }

                    //qeuryMax();
                    //queryInnerA();
                    //queryInnerB();
                    //queryCarton();
                    //queryExport();
                    //lTotal.Text = "Total: " + qty_current.ToString();
                    //total = 0;
                    //inner_count = 0;
                    //carton_count = 0;
                    //carton_need = 0;
                    //export_need = 0;
                    //Invoke((MethodInvoker)delegate { btnStart.Hide(); });
                    //tbKanban.ReadOnly = true;
                    //if (typeCheck() == 3)
                    //{
                    //    if (qty_current <= qty_kanban)
                    //    {
                    //        export_need++;
                    //        Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                    //    }
                    //    else
                    //    {
                    //        if (qty_current % qty_kanban == 0)
                    //        {
                    //            export_need = qty_current / qty_kanban;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                    //        }
                    //        else
                    //        {
                    //            export_need = qty_current / qty_kanban;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                    //            if ((qty_current % qty_kanban) < qty_kanban)
                    //            {
                    //                export_need++;
                    //                Invoke((MethodInvoker)delegate { lNeedExport.Text = "Need: " + export_need; });
                    //            }
                    //        }
                    //    }
                    //}
                    //else if (typeCheck() == 4)
                    //{
                    //    if (qty_current % innerbox_max == 0)
                    //    {
                    //        carton_need = qty_current / innerbox_max;
                    //        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                    //    }
                    //    else
                    //    {
                    //        carton_need = qty_current / innerbox_max;
                    //        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                    //        if ((qty_current % innerbox_max) < innerbox_max)
                    //        {
                    //            carton_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_need; });
                    //        }
                    //    }
                    //}
                    //yellowAlarm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }//Wait
        //Data Receiver
        private void dataReceiverRFIDLogin(object sender, SerialDataReceivedEventArgs e)
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
                MessageBox.Show(s_buffer);  //TEST
                Invoke((MethodInvoker)delegate { tbLogin.Text = s_buffer; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Waittest
        private void dataReceiverRFIDAccount(object sender, SerialDataReceivedEventArgs e)
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
                MessageBox.Show(s_buffer);  //TEST
                Invoke((MethodInvoker)delegate { tbDBTagpass.Text = s_buffer; });
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
                input_value = cutStr(input_value);
                Invoke((MethodInvoker)delegate { tbInnerBoxA.Text = input_value; });
                inner_a_master = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                if (input_value.Equals(inner_a_master))
                {
                    if (total < qty)
                    {
                        innerbox_counter++;
                        total++;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Inner Box A');");
                        if (typeCheck() == 1)
                        {
                            Invoke((MethodInvoker)delegate { lInnerBox.Text = "Inner Box: " + innerbox_counter + " / " + innerbox_max; });
                            if (innerbox_counter % innerbox_max == 0 || total == qty)
                            {
                                portsCloser();
                                port3Open();
                                Invoke((MethodInvoker)delegate { pArrow1.Visible = true; });
                                port3IsOn = true;
                            }
                        }
                        else if (typeCheck() == 2)
                        {
                            Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + innerbox_counter + " / " + cartonbox_max; });
                            if (innerbox_counter % cartonbox_max == 0 || total == qty)
                            {
                                portsCloser();
                                port4Open();
                                port4IsOn = true;
                                Invoke((MethodInvoker)delegate { pArrow2.Visible = true; });
                            }
                        }
                        _sound_player1.Play();
                    }
                    //if(qty_current - inner_count > 0)
                    //{
                    //    //Log
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //    //db Count per day
                    //    insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Inner Box');");

                    //    inner_count++;
                    //    total = qty_current - inner_count;
                    //    Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total.ToString(); });

                    //    if (typeCheck() == 1)
                    //    {
                    //        if (inner_count % innerbox_max == 0)
                    //        {
                    //            carton_need = (inner_count / innerbox_max) - carton_scanned;
                    //            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString() + " Scanned: " + carton_scanned.ToString(); });
                    //        }
                    //        else if (qty_current - inner_count == 0)//Fraction
                    //        {
                    //            carton_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString() + " Scanned: " + carton_scanned.ToString(); });
                    //        }
                    //    }
                    //    else if (typeCheck() == 2)
                    //    {
                    //        if (inner_count % cartonbox_max == 0)
                    //        {
                    //            export_need = (inner_count / cartonbox_max) - export_scanned;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString() + " Scanned: " + export_scanned.ToString(); });
                    //        }
                    //        else if (qty_current - inner_count == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString() + " Scanned: " + export_scanned.ToString(); });
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //}
                }
                else
                {
                    portsCloser();
                    Invoke((MethodInvoker)delegate { gbInnerA.BackColor = Color.Crimson; });
                    _sound_player4.PlayLooping();
                    portAlarm = 1;
                    redAlarm();
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
                input_value = cutStr(input_value);
                Invoke((MethodInvoker)delegate { tbInnerBoxB.Text = input_value; });
                inner_b_master = WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster;
                if (input_value.Equals(inner_b_master))
                {
                    if (total < qty)
                    {
                        innerbox_counter++;
                        total++;
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Inner Box B');");
                        if (typeCheck() == 1)
                        {
                            Invoke((MethodInvoker)delegate { lInnerBox.Text = "Inner Box: " + innerbox_counter + " / " + innerbox_max; });
                            if (innerbox_counter % innerbox_max == 0 || total == qty)
                            {
                                portsCloser();
                                port3Open();
                                Invoke((MethodInvoker)delegate { pArrow1.Visible = true; });
                                port3IsOn = true;
                            }
                        }
                        else if (typeCheck() == 2)
                        {
                            Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + innerbox_counter + " / " + cartonbox_max; });
                            if (innerbox_counter % cartonbox_max == 0 || total == qty)
                            {
                                portsCloser();
                                port4Open();
                                Invoke((MethodInvoker)delegate { pArrow2.Visible = true; });
                                port4IsOn = true;
                            }
                        }
                        _sound_player1.Play();
                    }
                    //if (qty_current - inner_count > 0)
                    //{
                    //    //Log
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //    insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Inner Box');");

                    //    inner_count++;
                    //    total = qty_current - inner_count;
                    //    Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total.ToString(); });

                    //    if (typeCheck() == 1)
                    //    {
                    //        if (inner_count % innerbox_max == 0)
                    //        {
                    //            carton_need = (inner_count / innerbox_max) - carton_scanned;
                    //            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString() + " Scanned: " + carton_scanned.ToString(); });
                    //        }
                    //        else if (qty_current - inner_count == 0)//Fraction
                    //        {
                    //            carton_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString() + " Scanned: " + carton_scanned.ToString(); });
                    //        }
                    //    }
                    //    else if (typeCheck() == 2)
                    //    {
                    //        if (inner_count % cartonbox_max == 0)
                    //        {
                    //            export_need = (inner_count / cartonbox_max) - export_scanned;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString() + " Scanned: " + export_scanned.ToString(); });
                    //        }
                    //        else if (qty_current - inner_count == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString() + " Scanned: " + export_scanned.ToString(); });
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //}
                }
                else
                {
                    portsCloser();
                    Invoke((MethodInvoker)delegate { gbInnerB.BackColor = Color.Crimson; });
                    _sound_player4.PlayLooping();
                    portAlarm = 2;
                    redAlarm();
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
                input_value = cutStr(input_value);
                Invoke((MethodInvoker)delegate { tbCartonBox.Text = input_value; });
                carton_master = WindowsForms_packing_line.Properties.Settings.Default.CartonMaster;
                if (input_value.Equals(carton_master))
                {
                    if (typeCheck() == 1 && cartonbox_counter < cartonbox_max)
                    {
                        cartonbox_counter++;
                        innerbox_counter = 0;
                        Invoke((MethodInvoker)delegate
                        {
                            lInnerBox.Text = "Inner Box: " + innerbox_counter + " / " + innerbox_max;
                            lCartonBox.Text = "Carton Box: " + cartonbox_counter + " / " + cartonbox_max;

                        });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Carton Box');");
                        port3IsOn = false;
                        
                        if (cartonbox_counter % cartonbox_max == 0 ||total == qty)
                        {
                            portsCloser();
                            port4Open();
                            Invoke((MethodInvoker)delegate { pArrow1.Visible = false; pArrow2.Visible = true; });
                            port4IsOn = true;
                        }
                        else
                        {
                            portsCloser();
                            port1Open();
                            port2Open();
                            Invoke((MethodInvoker)delegate { pArrow1.Visible = false; });
                        }
                    }
                    else if (typeCheck() == 4)
                    {
                        cartonbox_counter++;
                        Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + cartonbox_counter + " / " + cartonbox_max; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Carton Box');");
                        total -= innerbox_max;
                        if (total < 0) { total = 0; }
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                        if (cartonbox_counter % cartonbox_max == 0 || total == 0)
                        {
                            portsCloser();
                            port4Open();
                            Invoke((MethodInvoker)delegate { pArrow1.Visible = false; });
                            Invoke((MethodInvoker)delegate { pArrow2.Visible = true; });
                            port4IsOn = true;
                        }
                    }
                    _sound_player2.Play();
                    //if (carton_need > 0)
                    //{
                    //    //Log
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tCarton Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //    carton_scanned++;
                    //    carton_need--;
                    //    Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need + " Scanned: " + carton_scanned; });
                    //    insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Carton Box');");

                    //    if (typeCheck() == 1)
                    //    {
                    //        carton_count++;
                    //        if (carton_count % cartonbox_max == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need + " Scanned: " + export_scanned; });
                    //        }
                    //        else if (qty_current - inner_count == 0 & carton_need == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need + " Scanned: " + export_scanned; });
                    //        }
                    //    }
                    //    else if (typeCheck() == 4)
                    //    {
                    //        inner_count += innerbox_max;
                    //        total = qty_current - inner_count;
                    //        if (total < 0) { total = 0; }
                    //        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total.ToString(); });

                    //        carton_count++;
                    //        if (carton_count % cartonbox_max == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                    //        }
                    //        else if (total == 0 && carton_need == 0)
                    //        {
                    //            export_need++;
                    //            Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need; });
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //}
                }
                else
                {
                    portsCloser();
                    Invoke((MethodInvoker)delegate { gbCarton.BackColor = Color.Crimson; });
                    _sound_player4.PlayLooping();
                    portAlarm = 3;
                    redAlarm();
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
                if (checkVerticalBar(input_value))
                {
                    string[] kanban_array = input_value.Split('|');
                    input_value = kanban_array[2];
                }
                Invoke((MethodInvoker)delegate { tbExportBox.Text = input_value; });
                export_master = WindowsForms_packing_line.Properties.Settings.Default.ExportMaster;
                if (input_value.Equals(export_master))
                {
                    if (typeCheck() == 1 && (cartonbox_counter == cartonbox_max || total == qty))
                    {
                        exportbox_counter++;
                        Invoke((MethodInvoker)delegate { lExportBox.Text = "Export Box: " + exportbox_counter; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Export Box');");
                        cartonbox_counter = 0;
                        Invoke((MethodInvoker)delegate { lCartonBox.Text = "Carton Box: " + cartonbox_counter + " / " + innerbox_max; });
                        port4IsOn = false;
                        Invoke((MethodInvoker)delegate { pArrow2.Visible = false; });

                        if (total == qty)
                        {
                            portsCloser();
                            //reset
                            //MessageBox.Show(qty.ToString(), "type1");
                            updateActualTable(kanban_master, qty);
                            updateActualTable_NotEdit(kanban_master, qty);
                            Invoke((MethodInvoker)delegate { resetDefault(); });
                        }
                        else
                        {
                            portsCloser();
                            port1Open();
                            port2Open();
                        }
                    }
                    else if (typeCheck() == 2 && (innerbox_counter == cartonbox_max || total == qty))
                    {
                        exportbox_counter++;
                        Invoke((MethodInvoker)delegate { lExportBox.Text = "Export Box: " + exportbox_counter; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Export Box');");
                        innerbox_counter = 0;
                        Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + innerbox_counter + " / " + cartonbox_max; });
                        port4IsOn = false;
                        Invoke((MethodInvoker)delegate { pArrow2.Visible = false; });

                        if (total == qty)
                        {
                            portsCloser();
                            //reset
                            //MessageBox.Show(qty.ToString(), "type2");
                            updateActualTable(kanban_master, qty);
                            updateActualTable_NotEdit(kanban_master, qty);
                            Invoke((MethodInvoker)delegate { resetDefault(); });
                        }
                        else
                        {
                            portsCloser();
                            port1Open();
                            port2Open();
                        }
                    }
                    else if (typeCheck() == 3 && total > 0)
                    {
                        exportbox_counter++;
                        Invoke((MethodInvoker)delegate { lExportBox.Text = "Export Box: " + exportbox_counter; });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Export Box');");
                        total -= innerbox_max; //cartonbox_max = eb max, innerbox_max = ob max
                        if (total < innerbox_max) //cartonbox_max = eb max, innerbox_max = ob max
                        {
                            if (total < 0) { total = 0; }
                            Invoke((MethodInvoker)delegate { lCartonBox.Text = "Max: " + total + " / " + innerbox_max; }); //cartonbox_max = eb max, innerbox_max = ob max
                        }
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; });
                        if (total <= 0)
                        {
                            if (total < 0) { total = 0; Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total; }); }
                            Invoke((MethodInvoker)delegate { pArrow2.Visible = false; });
                            portsCloser();
                            //reset
                            //MessageBox.Show(qty.ToString(), "type3");
                            updateActualTable(kanban_master, qty);
                            updateActualTable_NotEdit(kanban_master, qty);
                            Invoke((MethodInvoker)delegate { resetDefault(); });
                        }
                    }
                    else if (typeCheck() == 4 && cartonbox_counter == cartonbox_max || total == 0)
                    {
                        cartonbox_counter = 0;
                        exportbox_counter++;
                        Invoke((MethodInvoker)delegate
                        {
                            lCartonBox.Text = "Max: " + cartonbox_counter + " / " + cartonbox_max;
                            lExportBox.Text = "Export Box: " + exportbox_counter;
                        });
                        insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Export Box');");
                        port4IsOn = false;
                        Invoke((MethodInvoker)delegate { pArrow2.Visible = false; });

                        if (total == 0)
                        {
                            portsCloser();
                            //reset
                            //MessageBox.Show(qty.ToString(), "type4");
                            updateActualTable(kanban_master, qty);
                            updateActualTable_NotEdit(kanban_master, qty);
                            Invoke((MethodInvoker)delegate { resetDefault(); });
                        }
                        else
                        {
                            Invoke((MethodInvoker)delegate { pArrow1.Visible = true; });
                            portsCloser();
                            port3Open();
                            port3IsOn = true;
                        }
                    }
                    _sound_player3.Play();
                    //if (export_need > 0) //type1-2
                    //{
                    //    //Log
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add(input_value + "\tExport Box"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //    export_scanned++;
                    //    export_need--;
                    //    Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need + " Scanned: " + export_scanned; });
                    //    insertCountperday("INSERT INTO `countperday`(kanban, countFrom) VALUES('" + kanban_master + "', 'Export Box');");

                    //    if (typeCheck() == 3)
                    //    {
                    //        inner_count += cartonbox_max;
                    //        total = qty_current - inner_count;
                    //        if (total < 0) { total = 0; }
                    //        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + total.ToString(); });
                    //    }
                    //    if (export_need == 0 && total == 0 && inner_count != 0)
                    //    {
                    //        portsCloser();
                    //        Invoke((MethodInvoker)delegate { lbLog.Items.Add("Need to reset Kanban."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //        MessageBox.Show("Alarm reset this Kanban! ");
                    //        updateActualTable(kanban_master, qty_current); //Update Actual Table
                    //        Invoke((MethodInvoker)delegate { lbLog.Items.Add("Actual Table has been updated."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //        inner_count = 0;
                    //    }
                    //}
                    //else
                    //{
                    //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("do not count."); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                    //}
                }
                else
                {
                    portsCloser();
                    Invoke((MethodInvoker)delegate { gbExport.BackColor = Color.Crimson; });
                    portAlarm = 4;
                    _sound_player4.PlayLooping();
                    redAlarm();
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
            if (pauseIsOn == true)
            {
                pauseIsOn = false;
                portsCloser();
                if (typeCheck() == 1)
                {
                    if (port3IsOn)
                    {
                        port3Open();
                    }
                    else if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port1Open();
                        port2Open();
                    }
                }
                else if (typeCheck() == 2)
                {
                    if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port1Open();
                        port2Open();
                    }
                }
                else if (typeCheck() == 3)
                {
                    port4Open();
                }
                else if (typeCheck() == 4)
                {
                    if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port3Open();
                    }
                }
            }
            else
            {
                pauseIsOn = true;
                portsCloser();
            }
            //if (inner_count > 0)
            //{
            //    inner_count--;
            //    Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty_current - inner_count).ToString(); });
            //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\tInner Box A"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
            //    deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Inner Box' LIMIT 1;");
            //    if (typeCheck() == 1)
            //    {
            //        if (inner_count % innerbox_max == 0 && carton_scanned > 0)
            //        {
            //            carton_scanned--;
            //            deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Carton Box' LIMIT 1;");//Not OK
            //        }
            //        carton_need = (inner_count / innerbox_max) - carton_scanned;
            //        if (carton_need < 0) { carton_need = 0; }
            //        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need.ToString() + " Scanned: " + carton_scanned; });
            //    }
            //    else if (typeCheck() == 2)
            //    {

            //        if (inner_count % cartonbox_max == 0 && export_scanned > 0)
            //        {
            //            export_scanned--;
            //            deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Export Box' LIMIT 1;");//Not OK
            //        }
            //        export_need = (inner_count / cartonbox_max) - export_scanned;
            //        if (export_need < 0) { export_need = 0; }
            //        Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need.ToString() + " Scanned: " + export_scanned; });
            //    }
            //}
            //else if (typeCheck() == 3 || typeCheck() == 4)
            //{

            //}
        }
        private void btnDecreaseInnerB_Click(object sender, EventArgs e)
        {
            if (pauseIsOn == true)
            {
                pauseIsOn = false;
                portsCloser();
            }
            else
            {
                pauseIsOn = true;
                portsCloser();
                if (typeCheck() == 1)
                {
                    if (port3IsOn)
                    {
                        port3Open();
                    }
                    else if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port1Open();
                        port2Open();
                    }
                }
                else if (typeCheck() == 2)
                {
                    if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port1Open();
                        port2Open();
                    }
                }
                else if (typeCheck() == 3)
                {
                    port4Open();
                }
                else if (typeCheck() == 4)
                {
                    if (port4IsOn)
                    {
                        port4Open();
                    }
                    else
                    {
                        port3Open();
                    }
                }
            }
            //if (inner_count > 0)
            //{
            //    inner_count--;
            //    Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + (qty_current - inner_count).ToString(); });
            //    Invoke((MethodInvoker)delegate { lbLog.Items.Add("Delete 1 Item!\tInner Box B"); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
            //    deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Inner Box' ORDER BY ID DESC LIMIT 1;");
            //    if (typeCheck() == 1)
            //    {
            //        if (inner_count % innerbox_max == 0 && carton_scanned > 0)
            //        {
            //            carton_scanned--;
            //            deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Carton Box' LIMIT 1;");//Not OK
            //        }
            //        carton_need = (inner_count / innerbox_max) - carton_scanned;
            //        if (carton_need < 0) { carton_need = 0; }
            //        Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Scan: " + carton_need + " Scanned: " + carton_scanned; });
            //    }
            //    else if (typeCheck() == 2)
            //    {
            //        if (inner_count % cartonbox_max == 0 && export_scanned > 0)
            //        {
            //            export_scanned--;
            //            deleteCountperday("DELETE FROM countperday WHERE countFrom = 'Export Box' LIMIT 1;");//Not OK
            //        }
            //        export_need = (inner_count / cartonbox_max) - export_scanned;
            //        if (export_need < 0) { export_need = 0; }
            //        Invoke((MethodInvoker)delegate { lNeedExport.Text = "Scan: " + export_need + " Scanned: " + export_scanned; });
            //    }
            //}
            //else if (typeCheck() == 3 || typeCheck() == 4)
            //{

            //}
        }
        //Close and Open port 1-4 Checker page
        private void lIsPort1Open_Click(object sender, EventArgs e)
        {
            try
            {
                if (port1 != null)
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
        public void port1Open()
        {
            try
            {
                if (port1 == null)
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port1 != "")
                    {
                        port1 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port1, WindowsForms_packing_line.Properties.Settings.Default.Baudrate1, WindowsForms_packing_line.Properties.Settings.Default.Paritybit1, WindowsForms_packing_line.Properties.Settings.Default.Datasize1, WindowsForms_packing_line.Properties.Settings.Default.Stopbits1);
                        port1.Open();
                        port1.DataReceived += new SerialDataReceivedEventHandler(dataReceiver1);
                        if (port1.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort1Open.Text = "Port1: Online ";
                                lIsPort1Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
                else
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port1 != "")
                    {
                        port1 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port1, WindowsForms_packing_line.Properties.Settings.Default.Baudrate1, WindowsForms_packing_line.Properties.Settings.Default.Paritybit1, WindowsForms_packing_line.Properties.Settings.Default.Datasize1, WindowsForms_packing_line.Properties.Settings.Default.Stopbits1);
                        port1.Open();
                        port1.DataReceived += new SerialDataReceivedEventHandler(dataReceiver1);
                        if (port1.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort1Open.Text = "Port1: Online ";
                                lIsPort1Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port1 Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Wait
        public void port2Open()
        {
            try
            {
                if (port2 == null)
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port2 != "")
                    {
                        port2 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port2, WindowsForms_packing_line.Properties.Settings.Default.Baudrate2, WindowsForms_packing_line.Properties.Settings.Default.Paritybit2, WindowsForms_packing_line.Properties.Settings.Default.Datasize2, WindowsForms_packing_line.Properties.Settings.Default.Stopbits2);
                        port2.Open();
                        port2.DataReceived += new SerialDataReceivedEventHandler(dataReceiver2);
                        if (port2.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort2Open.Text = "Port2: Online ";
                                lIsPort2Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
                else
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port2 != "")
                    {
                        port2 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port2, WindowsForms_packing_line.Properties.Settings.Default.Baudrate2, WindowsForms_packing_line.Properties.Settings.Default.Paritybit2, WindowsForms_packing_line.Properties.Settings.Default.Datasize2, WindowsForms_packing_line.Properties.Settings.Default.Stopbits2);
                        port2.Open();
                        port2.DataReceived += new SerialDataReceivedEventHandler(dataReceiver2);
                        if (port2.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort2Open.Text = "Port2: Online ";
                                lIsPort2Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port2 Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Wait
        public void port3Open()
        {
            try
            {
                if (port3 == null)
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port3 != "")
                    {
                        port3 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port3, WindowsForms_packing_line.Properties.Settings.Default.Baudrate3, WindowsForms_packing_line.Properties.Settings.Default.Paritybit3, WindowsForms_packing_line.Properties.Settings.Default.Datasize3, WindowsForms_packing_line.Properties.Settings.Default.Stopbits3);
                        port3.Open();
                        port3.DataReceived += new SerialDataReceivedEventHandler(dataReceiver3);
                        if (port3.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort3Open.Text = "Port3: Online ";
                                lIsPort3Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
                else
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port3 != "")
                    {
                        port3 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port3, WindowsForms_packing_line.Properties.Settings.Default.Baudrate3, WindowsForms_packing_line.Properties.Settings.Default.Paritybit3, WindowsForms_packing_line.Properties.Settings.Default.Datasize3, WindowsForms_packing_line.Properties.Settings.Default.Stopbits3);
                        port3.Open();
                        port3.DataReceived += new SerialDataReceivedEventHandler(dataReceiver3);
                        if (port3.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort3Open.Text = "Port3: Online ";
                                lIsPort3Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port3 Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Wait
        public void port4Open()
        {
            try
            {
                if (port4 == null)
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port4 != "")
                    {
                        port4 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port4, WindowsForms_packing_line.Properties.Settings.Default.Baudrate4, WindowsForms_packing_line.Properties.Settings.Default.Paritybit4, WindowsForms_packing_line.Properties.Settings.Default.Datasize4, WindowsForms_packing_line.Properties.Settings.Default.Stopbits4);
                        port4.Open();
                        port4.DataReceived += new SerialDataReceivedEventHandler(dataReceiver4);
                        if (port4.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort4Open.Text = "Port4: Online ";
                                lIsPort4Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
                else
                {
                    if (WindowsForms_packing_line.Properties.Settings.Default.Port4 != "")
                    {
                        port4 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port4, WindowsForms_packing_line.Properties.Settings.Default.Baudrate4, WindowsForms_packing_line.Properties.Settings.Default.Paritybit4, WindowsForms_packing_line.Properties.Settings.Default.Datasize4, WindowsForms_packing_line.Properties.Settings.Default.Stopbits4);
                        port4.Open();
                        port4.DataReceived += new SerialDataReceivedEventHandler(dataReceiver4);
                        if (port4.IsOpen)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lIsPort4Open.Text = "Port4: Online ";
                                lIsPort4Open.BackColor = System.Drawing.Color.Green;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port4 Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Wait
        private void btnResetKB_Click(object sender, EventArgs e)
        {
            if (portAlarm != 0)
            {
                alarmAuth();
                if (Authentication.alarm_turn_off == true)
                {
                    _sound_player4.Stop();
                    yellowAlarm();
                    if (portAlarm == 1)
                    {
                        Invoke((MethodInvoker)delegate { gbInnerA.BackColor = Color.Transparent; });
                        port1Open();
                        port2Open();
                    }
                    else if (portAlarm == 2)
                    {
                        Invoke((MethodInvoker)delegate { gbInnerB.BackColor = Color.Transparent; });
                        port2Open();
                        port1Open();
                    }
                    else if (portAlarm == 3)
                    {
                        Invoke((MethodInvoker)delegate { gbCarton.BackColor = Color.Transparent; });
                        port3Open();
                    }
                    else if (portAlarm == 4)
                    {
                        Invoke((MethodInvoker)delegate { gbExport.BackColor = Color.Transparent; });
                        port4Open();
                    }
                    portAlarm = 0;
                }
            }
            //DialogResult dialog_result = MessageBox.Show("Are you sure to reset Kanban?", "Reset Kanban", MessageBoxButtons.YesNo);
            //if (dialog_result == DialogResult.Yes)
            //{
            //    alarmAuth();
            //    if (Authentication.alarm_turn_off == true)
            //    {
            //        //resetDefault();
            //    }
            //}
        }//OK
        //Actual Table
        private void btnActualTableRefresh_Click(object sender, EventArgs e)
        {
            refreshDGVAcutalTable();
        }
        private void btnCountPerDay_Click(object sender, EventArgs e)
        {
            Countperday form_countperday = new Countperday();
            form_countperday.Show();
        }
        //Edit Master page
        //Create kanban button
        private void btnCreateMaster_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Are you sure to insert new tool setting?", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                string TABLE = "modelmaster";
                string INSERT_STR = " (kanban, modelName, bc1, bc2, bc3, bc4, obMax, ebMax) VALUES('" + tbKBSearch.Text + "', '" + tbDBModel.Text + "', '" + tbDBInnerA.Text + "', '" + tbDBInnerB.Text + "', '" + tbDBCarton.Text + "', '" + tbDBExport.Text + "', '" + tbDBInnerMax.Text + "', '" + tbDBCartonMax.Text + "');";
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
                if (selected_kanban_name == "")
                {
                    MessageBox.Show("Error: Please select Kanban before update database!", "Database");
                }
                else
                {
                    dbUpdate("UPDATE modelmaster SET kanban = '" + tbKBSearch.Text + "', modelName = '" + tbDBModel.Text + "', bc1 = '" + tbDBInnerA.Text + "', bc2 = '" + tbDBInnerB.Text + "', bc3 = '" + tbDBCarton.Text + "', bc4 = '" + tbDBExport.Text + "', obMax = '" + tbDBInnerMax.Text + "', ebMax = '" + tbDBCartonMax.Text + "' WHERE kanban = '" + selected_kanban_name + "';");
                    dbSearchKanban();
                    selected_kanban_name = "";
                }
            }
        }//OK
        //Delete kanban button
        private void btnDeleteMaster_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Delete databasse", "Database", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                if (selected_kanban_name == "")
                {
                    MessageBox.Show("Error: Please select Kanban before delete database!", "Database");
                }
                else
                {
                    dbDelete("DELETE FROM `modelmaster` WHERE kanban = '" + selected_kanban_name + "';");
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
            selected_kanban_name = selected_item.SubItems[0].Text;
            tbKBSearch.Text = selected_item.SubItems[0].Text;
            tbDBModel.Text = selected_item.SubItems[1].Text;
            tbDBInnerA.Text = selected_item.SubItems[2].Text;
            tbDBInnerB.Text = selected_item.SubItems[3].Text;
            tbDBCarton.Text = selected_item.SubItems[4].Text;
            tbDBExport.Text = selected_item.SubItems[5].Text;
            tbDBInnerMax.Text = selected_item.SubItems[6].Text;
            tbDBCartonMax.Text = selected_item.SubItems[7].Text;
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
        private void btnAccountRFID_Click(object sender, EventArgs e)
        {
            try
            {
                portRFID.Open();
                portRFID.DataReceived += new SerialDataReceivedEventHandler(dataReceiverRFIDAccount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            tbDBTagpass.Focus();
        }//Waittest
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (tbDBTagpass.Text != "")
            {
                DialogResult dialog_result = MessageBox.Show("Are you sure to insert new Account?", "Database", MessageBoxButtons.YesNo);
                if (dialog_result == DialogResult.Yes)
                {
                    dbCreate("INSERT INTO account (tagpass, operatorID, fname, lname, position) VALUES('" + tbDBTagpass.Text + "', '" + tbDBOperatorID.Text + "', '" + tbDBName.Text + "', '" + tbDBSurname.Text + "', '" + cbDBPosition.Text + "');");
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
                else if (tbDBOperatorID.Text != "" && tbDBName.Text != "" && tbDBSurname.Text != "" && cbDBPosition.Text != "")
                {
                    dbUpdate("UPDATE account SET tagpass = '" + tbDBTagpass.Text + "', operatorID = '" + tbDBOperatorID.Text + "', fname = '" + tbDBName.Text + "', lname = '" + tbDBSurname.Text + "', position = '" + cbDBPosition.Text + "' WHERE tagpass = '" + selected_account_tagpass + "';");
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
                    dbDelete("DELETE FROM `account` WHERE tagpass = '" + selected_account_tagpass + "';");
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
                string TABLE = "account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE tagpass LIKE '%" + tbDBTagpass.Text + "%' ORDER BY CAST(tagpass AS UNSIGNED);";
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
                        ListViewItem list = new ListViewItem(dr["tagpass"].ToString());
                        list.SubItems.Add(dr["operatorID"].ToString());
                        list.SubItems.Add(dr["fname"].ToString());
                        list.SubItems.Add(dr["lname"].ToString());
                        list.SubItems.Add(dr["position"].ToString());
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
        private void tbDBOperatorID_TextChanged(object sender, EventArgs e)
        {
            if (tbDBOperatorID.Text == "")
            {
                refreshListViewAccount();
                tbEditAccountClear();
            }
            else
            {
                lvAccount.Items.Clear();
                string TABLE = "account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE operatorID LIKE '%" + tbDBOperatorID.Text + "%' ORDER BY CAST(operatorID AS UNSIGNED);";
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
                        ListViewItem list = new ListViewItem(dr["tagpass"].ToString());
                        list.SubItems.Add(dr["operatorID"].ToString());
                        list.SubItems.Add(dr["fname"].ToString());
                        list.SubItems.Add(dr["lname"].ToString());
                        list.SubItems.Add(dr["position"].ToString());
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
        }//Waittest
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
                string kb = "";
                string TABLE = "modelmaster";
                string queryList = "SELECT * FROM " + TABLE + " WHERE kanban LIKE '%" + tbKBSearch.Text + "%' ORDER BY CAST(kanban AS UNSIGNED);";
                if (checkVerticalBar(tbKBSearch.Text))
                {
                    string[] kanban_array = tbKBSearch.Text.Split('|');
                    kb = kanban_array[2];
                    queryList = "SELECT * FROM " + TABLE + " WHERE kanban LIKE '%" + kb + "%' ORDER BY CAST(kanban AS UNSIGNED);";
                    tbKBSearch.Text = kb;
                    lvModelMaster.Items.Clear();
                }
                //Database display Search data from table
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
                        ListViewItem list = new ListViewItem(dr["kanban"].ToString());
                        list.SubItems.Add(dr["modelName"].ToString());
                        list.SubItems.Add(dr["bc1"].ToString());
                        list.SubItems.Add(dr["bc2"].ToString());
                        list.SubItems.Add(dr["bc3"].ToString());
                        list.SubItems.Add(dr["bc4"].ToString());
                        list.SubItems.Add(dr["obMax"].ToString());
                        list.SubItems.Add(dr["ebMax"].ToString());
                        //list.SubItems.Add(dr["CartonMax"].ToString());
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
                    Invoke((MethodInvoker)delegate
                    {
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
                MessageBox.Show(ex.Message, "Ports Closer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        public void portsOtherClose()
        {
            try
            {
                if (portTowerLamp != null)
                {
                    portTowerLamp.Close();
                    Invoke((MethodInvoker)delegate
                    {
                        lTowerLamp.Text = "Tower Lamp: Disonnected";
                        lTowerLamp.ForeColor = Color.Red;
                    });
                }
                if (portPDFLink != null)
                {
                    portPDFLink.Close();
                    Invoke((MethodInvoker)delegate
                    {
                        lLinkPDF.Text = "PDF Link: Disconnected";
                        lLinkPDF.ForeColor = Color.Red;
                    });
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
                        Invoke((MethodInvoker)delegate
                        {
                            lIsPort1Open.Text = "Port1: Online ";
                            lIsPort1Open.BackColor = System.Drawing.Color.Green;
                        });
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
                        Invoke((MethodInvoker)delegate
                        {
                            lIsPort2Open.Text = "Port2: Online ";
                            lIsPort2Open.BackColor = System.Drawing.Color.Green;
                        });
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
                        Invoke((MethodInvoker)delegate
                        {
                            lIsPort3Open.Text = "Port3: Online ";
                            lIsPort3Open.BackColor = System.Drawing.Color.Green;
                        });
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
                        Invoke((MethodInvoker)delegate
                        {
                            lIsPort4Open.Text = "Port4: Online ";
                            lIsPort4Open.BackColor = System.Drawing.Color.Green;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//OK
        public void portsOtherOpen()
        {
            try
            {
                if (cbPortTL.Text != "")
                {
                    portTowerLamp = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.PortTL, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    portTowerLamp.Open();
                    if (portTowerLamp.IsOpen)
                    {
                        lTowerLamp.Text = "Tower Lamp: Connected";
                        lTowerLamp.ForeColor = Color.Green;
                    }
                }
                if (cbPortLink.Text != "")
                {
                    portPDFLink = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.PortLink, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    portPDFLink.Open();
                    if (portPDFLink.IsOpen)
                    {
                        lLinkPDF.Text = "PDF Link: Connected";
                        lLinkPDF.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void showSettingPorts()
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
        public void portInitial()
        {
            if (port1 == null)
            {
                port1 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port1, WindowsForms_packing_line.Properties.Settings.Default.Baudrate1, WindowsForms_packing_line.Properties.Settings.Default.Paritybit1, WindowsForms_packing_line.Properties.Settings.Default.Datasize1, WindowsForms_packing_line.Properties.Settings.Default.Stopbits1);
            }
            if (port2 == null)
            {
                port2 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port2, WindowsForms_packing_line.Properties.Settings.Default.Baudrate2, WindowsForms_packing_line.Properties.Settings.Default.Paritybit2, WindowsForms_packing_line.Properties.Settings.Default.Datasize2, WindowsForms_packing_line.Properties.Settings.Default.Stopbits2);
            }
            if (port3 == null)
            {
                port3 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port3, WindowsForms_packing_line.Properties.Settings.Default.Baudrate3, WindowsForms_packing_line.Properties.Settings.Default.Paritybit3, WindowsForms_packing_line.Properties.Settings.Default.Datasize3, WindowsForms_packing_line.Properties.Settings.Default.Stopbits3);
            }
            if (port4 == null)
            {
                port4 = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.Port4, WindowsForms_packing_line.Properties.Settings.Default.Baudrate4, WindowsForms_packing_line.Properties.Settings.Default.Paritybit4, WindowsForms_packing_line.Properties.Settings.Default.Datasize4, WindowsForms_packing_line.Properties.Settings.Default.Stopbits4);
            }
        }
        //SQL Connect, Get
        public void qeuryMax()
        {
            string queryList = "SELECT * FROM modelmaster WHERE kanban = '" + tbKanban.Text + "';";
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
                    innerbox_max = reader.GetInt32("obMax");     //Get InnerMax from db
                    cartonbox_max = reader.GetInt32("ebMax");   //Get CartonMax from db
                    //lNeedCarton.Text = "Scan: " + carton_need.ToString();   //Display - (Carton)Scan: 0
                    //lNeedExport.Text = "Scan: " + export_need.ToString();   //Display - (Export)Scan: 0
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
            string queryList = "SELECT * FROM modelmaster WHERE kanban = '" + tbKanban.Text + "';";
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
                    WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = reader.GetString("bc1");
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
            string queryList = "SELECT * FROM modelmaster WHERE kanban = '" + tbKanban.Text + "';";
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
                    WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = reader.GetString("bc2");
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
            string queryList = "SELECT * FROM modelmaster WHERE kanban = '" + tbKanban.Text + "';";
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
                    WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = reader.GetString("bc3");
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
            string queryList = "SELECT * FROM modelmaster WHERE kanban = '" + tbKanban.Text + "';";
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
                    WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = reader.GetString("bc4");
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
            string TABLE = "modelmaster";
            string queryList = "SELECT * FROM " + TABLE + " ORDER BY CAST(kanban AS UNSIGNED);";
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
                    ListViewItem list = new ListViewItem(dr["kanban"].ToString());
                    list.SubItems.Add(dr["modelName"].ToString());
                    list.SubItems.Add(dr["bc1"].ToString());
                    list.SubItems.Add(dr["bc2"].ToString());
                    list.SubItems.Add(dr["bc3"].ToString());
                    list.SubItems.Add(dr["bc4"].ToString());
                    list.SubItems.Add(dr["obMax"].ToString());
                    list.SubItems.Add(dr["ebMax"].ToString());
                    //list.SubItems.Add(dr["CartonMax"].ToString());
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
            string TABLE = "modelmaster";
            string queryList = "SELECT * FROM " + TABLE + " WHERE kanban = '" + tbKBSearch.Text + "';";
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
                    ListViewItem list = new ListViewItem(dr["kanban"].ToString());
                    list.SubItems.Add(dr["modelName"].ToString());
                    list.SubItems.Add(dr["bc1"].ToString());
                    list.SubItems.Add(dr["bc2"].ToString());
                    list.SubItems.Add(dr["bc3"].ToString());
                    list.SubItems.Add(dr["bc4"].ToString());
                    list.SubItems.Add(dr["obMax"].ToString());
                    list.SubItems.Add(dr["ebMax"].ToString());
                    //list.SubItems.Add(dr["CartonMax"].ToString());
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
            string TABLE = "account";
            string queryList = "SELECT * FROM " + TABLE + " ORDER BY CAST(tagpass AS UNSIGNED);";
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
                    ListViewItem list = new ListViewItem(dr["tagpass"].ToString());
                    list.SubItems.Add(dr["operatorID"].ToString());
                    list.SubItems.Add(dr["fname"].ToString());
                    list.SubItems.Add(dr["lname"].ToString());
                    list.SubItems.Add(dr["position"].ToString());
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
            string TABLE = "account";
            string queryList = "SELECT * FROM " + TABLE + " WHERE tagpass = '" + tbDBTagpass.Text + "';";
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
                    ListViewItem list = new ListViewItem(dr["tagpass"].ToString());
                    list.SubItems.Add(dr["operatorID"].ToString());
                    list.SubItems.Add(dr["fname"].ToString());
                    list.SubItems.Add(dr["lname"].ToString());
                    list.SubItems.Add(dr["position"].ToString());
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
        public void refreshDGVAcutalTable()
        {
            string TABLE = "actualcount";
            string queryList = "SELECT * FROM " + TABLE + " ORDER BY CAST(pn AS UNSIGNED);";
            MySqlConnection dbconnect = new MySqlConnection(connectStr);
            MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
            MySqlDataAdapter da = new MySqlDataAdapter(dbcommand);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGVActualTable.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }//Waittest
        public void updateActualTable(string kanban, int amount)
        {
            int db_count = 0;
            string TABLE = "actualcount";
            string queryList = "SELECT * FROM " + TABLE + " WHERE pn = '" + kanban + "';";
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
                    db_count = Int32.Parse(reader.GetString("count"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (db_count == 0)
            {
                TABLE = "`actualcount`(pn, count)";
                queryList = "INSERT INTO " + TABLE + " VALUES('" + kanban + "', '" + amount + "');";
                dbconnect = new MySqlConnection(connectStr);
                dbcommand = new MySqlCommand(queryList, dbconnect);
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
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
            else
            {
                amount += db_count;
                TABLE = "`actualcount`";
                queryList = "UPDATE " + TABLE + " SET count = '" + amount + "' WHERE pn = '" + kanban + "';";
                dbconnect = new MySqlConnection(connectStr);
                dbcommand = new MySqlCommand(queryList, dbconnect);
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
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
        }//Waittest
        public void updateActualTable_NotEdit(string kanban, int amount)
        {
            int db_count = 0;
            string TABLE = "actualcount_not_edit";
            string queryList = "SELECT * FROM " + TABLE + " WHERE pn = '" + kanban + "';";
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
                    db_count = Int32.Parse(reader.GetString("count"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (db_count == 0)
            {
                TABLE = "`actualcount_not_edit`(pn, count)";
                queryList = "INSERT INTO " + TABLE + " VALUES('" + kanban + "', '" + amount + "');";
                dbconnect = new MySqlConnection(connectStr);
                dbcommand = new MySqlCommand(queryList, dbconnect);
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
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
            else
            {
                amount += db_count;
                TABLE = "`actualcount_not_edit`";
                queryList = "UPDATE " + TABLE + " SET count = '" + amount + "' WHERE pn = '" + kanban + "';";
                dbconnect = new MySqlConnection(connectStr);
                dbcommand = new MySqlCommand(queryList, dbconnect);
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
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
        }//Waittest
        public void insertCountperday(string s)
        {
            MySqlConnection dbconnect = new MySqlConnection(Form1.connectStr);
            MySqlCommand dbcommand = new MySqlCommand(s, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }//Waittest
        public void deleteCountperday(string s)
        {
            MySqlConnection dbconnect = new MySqlConnection(Form1.connectStr);
            MySqlCommand dbcommand = new MySqlCommand(s, dbconnect);
            MySqlDataReader reader;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect.Open();
                reader = dbcommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
        }//Waittest
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
            hideTab(Actualtable, false);
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
            if (s == "operator")
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
            else if (s == "supervisor")
            {
                tbDBName.Enabled = false;
                tbDBSurname.Enabled = false;
                cbDBPosition.Enabled = false;
                btnCreateAccount.Hide();
                btnUpdateAccount.Hide();
                btnDeleteAccount.Hide();
            }
            else if (s == "admin")
            {

            }
            else
            {
                enableTab(Checker, false);
                hideTab(Settings, true);
                hideTab(Actualtable, true);
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
        public void Login()
        {
            try
            {
                if (portRFID != null && portRFID.IsOpen == true)
                {
                    string TABLE = "account";
                    string queryList = "SELECT * FROM " + TABLE + " WHERE tagpass = '" + tbLogin.Text + "';";
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
                            lOperatorID.Text = "Operator ID: " + reader.GetString("operatorID");
                            lOperatorName.Text = "Operator Name: " + reader.GetString("fname") + " " + reader.GetString("lname");
                            lPosition.Text = "Position: " + reader.GetString("position");
                            pLogin.Hide();
                            btnLogout.Show();
                            roleChecker(reader.GetString("position"));
                            tbKanban.Focus();
                            portsCloser();
                        }
                        tbLogin.SelectAll();
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
                }
                else
                {
                    string TABLE = "account";
                    string queryList = "SELECT * FROM " + TABLE + " WHERE tagpass = '" + tbLogin.Text + "';";
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
                            lOperatorID.Text = "Operator ID: " + reader.GetString("operatorID");
                            lOperatorName.Text = "Operator Name: " + reader.GetString("fname") + " " + reader.GetString("lname");
                            lPosition.Text = "Position: " + reader.GetString("position");
                            pLogin.Hide();
                            btnLogout.Show();
                            roleChecker(reader.GetString("position"));
                            tbKanban.Focus();
                            portsCloser();
                        }
                        tbLogin.SelectAll();
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Waittest
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
        public void resetDefault()
        {

            WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.InnerBMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.CartonMaster = "";
            WindowsForms_packing_line.Properties.Settings.Default.ExportMaster = "";
            //qty_current = 0;
            qty = 0;
            total = 0;
            innerbox_max = 0;
            cartonbox_max = 0;
            //inner_count = 0;
            //carton_count = 0;
            //carton_need = 0;
            //export_need = 0;
            //carton_scanned = 0;
            //export_scanned = 0;
            inner_a_master = "";
            inner_b_master = "";
            carton_master = "";
            export_master = "";
            kanban_master = "";
            tbKanban.Clear();
            tbModel.Clear();
            tbQTY.Clear();
            lMasterA.Text = "Master: -" + inner_a_master;
            lMasterB.Text = "Master: -" + inner_b_master;
            lMasterCarton.Text = "Master: -" + carton_master;
            lMasterExport.Text = "Master: -" + export_master;
            //lNeedCarton.Text = "Scan: " + carton_need.ToString();
            //lNeedExport.Text = "Scan: " + export_need.ToString();
            tbInnerBoxA.Clear();
            tbInnerBoxB.Clear();
            tbCartonBox.Clear();
            tbExportBox.Clear();
            innerbox_counter = 0;
            cartonbox_counter = 0;
            exportbox_counter = 0;
            lTotal.Text = "Total: 0";
            lInnerBox.Text = "Inner Box: " + innerbox_counter + " / " + innerbox_max;
            lCartonBox.Text = "Carton Box: " + cartonbox_counter + " / " + cartonbox_max;
            lExportBox.Text = "Export Box: " + exportbox_counter;
            tbKanban.ReadOnly = false;
            pArrow1.Visible = false;
            pArrow2.Visible = false;
            btnStart.BackColor = Color.MediumAquamarine;
            btnStart.Text = "START";
            switchIsOn = false;
            //lbLog.Items.Clear();
            //btnStart.Show();
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
        //Modbus TCP
        public void modbusConnect()
        {
            try
            {
                //modbus_tcp.Connect();
            }
            catch
            {

            }
        }//Unknown
        //Tower Lamp
        public void portTLOpen()
        {
            try
            {
                if (WindowsForms_packing_line.Properties.Settings.Default.PortTL != "")
                {
                    portTowerLamp = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.PortTL, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    portTowerLamp.Open();
                    lTowerLamp.Text = "Tower Lamp: Connected";
                    lTowerLamp.ForeColor = Color.Green;
                }
                else
                {
                    lTowerLamp.Text = "Tower Lamp: Disconnected";
                    lTowerLamp.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port Tower Lamp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pArrow1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Brushes.LimeGreen, 25);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            graphics.DrawLine(pen, 100, 60, 30, 60);
        }
        private void pArrow2_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Pen pen = new Pen(Brushes.LimeGreen, 25);
            pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            graphics.DrawLine(pen, 100, 60, 30, 60);
        }
        public void sendToUSB(colors c)
        {
            try
            {
                Thread.Sleep(60);
                if (portTowerLamp != null && cbPortTL.Text != "")
                {
                    if (colors.red == c)
                    {
                        byte[] buffer = new byte[] { 0X11 };
                        portTowerLamp.Write(buffer, 0, buffer.Length);
                    }
                    else if (colors.yellow == c)
                    {
                        byte[] buffer = new byte[] { 0X12 };
                        portTowerLamp.Write(buffer, 0, buffer.Length);
                    }
                    else if (colors.green == c)
                    {
                        byte[] buffer = new byte[] { 0X14 };
                        portTowerLamp.Write(buffer, 0, buffer.Length);
                    }
                    else if (colors.buzzer == c)
                    {
                        byte[] buffer = new byte[] { 0X18 };
                        portTowerLamp.Write(buffer, 0, buffer.Length);
                    }
                    else if (colors.reset == c)
                    {
                        byte[] buffer = new byte[] { 0X21 };
                        portTowerLamp.Write(buffer, 0, buffer.Length);
                        Thread.Sleep(60);
                        byte[] buffer1 = new byte[] { 0X22 };
                        portTowerLamp.Write(buffer1, 0, buffer.Length);
                        Thread.Sleep(60);
                        byte[] buffer2 = new byte[] { 0X24 };
                        portTowerLamp.Write(buffer2, 0, buffer.Length);
                        Thread.Sleep(60);
                        byte[] buffer3 = new byte[] { 0X28 };
                        portTowerLamp.Write(buffer3, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send to usb", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //PDF
        public void portPDFLinkOpen()
        {
            try
            {
                if (WindowsForms_packing_line.Properties.Settings.Default.PortLink != "")
                {
                    portPDFLink = new SerialPort(WindowsForms_packing_line.Properties.Settings.Default.PortLink, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    portPDFLink.Open();
                    lLinkPDF.Text = "PDF Link: Connected";
                    lLinkPDF.ForeColor = Color.Green;
                }
                else
                {
                    lLinkPDF.Text = "PDF Link: Disconnected";
                    lLinkPDF.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Port PDF Link", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void sendToRS232(string s)
        {
            Thread.Sleep(500);
            try
            {
                if (portPDFLink != null && portPDFLink.IsOpen == true)
                {
                    Thread.Sleep(60);
                    if (s != "")
                    {
                        portPDFLink.Write(s + Environment.NewLine);
                    }
                    else
                    {
                        portPDFLink.Write(tbKanban.Text + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Send to RS232", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void redAlarm()
        {
            Invoke((MethodInvoker)delegate
            {
                sendToUSB(colors.reset);
                sendToUSB(colors.red);
                sendToUSB(colors.buzzer);
            });
        }
        public async Task greenAlarm()
        {
            await Task.Run(() =>
            {
                Invoke((MethodInvoker)delegate
                {
                    sendToUSB(colors.reset);
                    sendToUSB(colors.green);
                });
            });
        }
        public void yellowAlarm()
        {
            Invoke((MethodInvoker)delegate
            {
                sendToUSB(colors.reset);
                sendToUSB(colors.yellow);
            });
        }
        public void Calculate()
        {
            bool isInteger = false;
            try
            {
                isInteger = int.TryParse(tbQTY.Text, out qty_current);
                if (!isInteger)
                {
                    MessageBox.Show("QTY is incorrect!", "Checker");
                }
                else
                {
                    tbKanban.ReadOnly = true;
                    qeuryMax();
                    queryInnerA();
                    queryInnerB();
                    queryCarton();
                    queryExport();
                    lTotal.Text = "Total: " + total;
                    lExportBox.Text = "Export Box: " + exportbox_counter;
                    if (typeCheck() == 1)
                    {
                        lInnerBox.Text = "Inner Box: " + innerbox_counter + " / " + innerbox_max;
                        lCartonBox.Text = "Carton Box: " + cartonbox_counter + " / " + cartonbox_max;
                    }
                    else if (typeCheck() == 2)
                    {
                        lInnerBox.Text = "Max: -";
                        lCartonBox.Text = "Max: " + innerbox_counter + " / " + cartonbox_max;
                    }
                    else if (typeCheck() == 3)
                    {
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + qty; });
                        lInnerBox.Text = "Max: -";
                        lCartonBox.Text = "Max: " + innerbox_max + " / " + innerbox_max; //cartonbox_max = eb max, innerbox_max = ob max
                    }
                    else if (typeCheck() == 4)
                    {
                        Invoke((MethodInvoker)delegate { lTotal.Text = "Total: " + qty; });
                        innerbox_counter += innerbox_max;
                        lInnerBox.Text = "Max: " + innerbox_counter + " / " + innerbox_max;
                        lCartonBox.Text = "Max: " + cartonbox_counter + " / " + cartonbox_max;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public string cutStr(string s)
        {
            char target = '~';
            char[] c_array = s.ToCharArray();
            int index = 0;
            foreach(char c in c_array)
            {
                if (c.Equals(target))
                {
                    break;
                }
                index++;
            }
            return s.Substring(0, index);
        }
        public bool checkVerticalBar(string str)
        {
            char target = '|';
            char[] c_array = str.ToCharArray();
            foreach(char c in c_array)
            {
                if (c.Equals(target))
                {
                    return true;
                }
            }
            return false;
        }

        //private List<ActualTable> getActualTable()
        //{
        //    var list = new List<ActualTable>();
        //    list.Add(new ActualTable()
        //    {
        //        No = 0,
        //        PartNo = "",
        //        Count = 0

        //    });
        //    return list;
        //}
    }
}