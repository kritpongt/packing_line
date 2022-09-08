﻿using System;
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

namespace WindowsForms_packing_line
{
    public partial class Form1 : Form
    {
        private SerialPort port1, port2, port3, port4;
        string connectStr = "server=" + WindowsForms_packing_line.Properties.Settings.Default.dbIPServer + ";port=3306;Database=packing_line_element;uid=root;pwd=;SslMode=none;";
        int qty, innerbox_max, cartonbox_max;   //get from db
        int cartonbox_rem;  //remaining carton box in export box
        int inner_count = 0 , carton_count = 0; //counter +1
        string inner_a_master;  //No. inner master
        string inner_b_master;  //No. inner master
        string carton_master;   //No. carton master
        string export_master;   //No. export master
        public Form1()
        {
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = FormBorderStyle.None;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //Ports Indicator(in ComboBoxes)
            string[] getPorts = SerialPort.GetPortNames();
            cbPort1.Items.AddRange(getPorts);
            cbPort1.Text = " -- Select Port -- ";
            cbPort2.Items.AddRange(getPorts);
            cbPort2.Text = " -- Select Port -- ";
            cbPort3.Items.AddRange(getPorts);
            cbPort3.Text = " -- Select Port -- ";
            cbPort4.Items.AddRange(getPorts);
            cbPort4.Text = " -- Select Port -- ";
        }
        //IF ports are selected then settings
        private void cbPort1_SelectedIndexChanged(object sender, EventArgs e)
        {
            port1 = new SerialPort(cbPort1.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            try
            {
                port1.Open();
                port1.DataReceived += new SerialDataReceivedEventHandler(dataReceiver1);
                if (port1.IsOpen)
                {
                    //Status: Online
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void cbPort2_SelectedIndexChanged(object sender, EventArgs e)
        {
            port2 = new SerialPort(cbPort2.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            try
            {
                port2.Open();
                port2.DataReceived += new SerialDataReceivedEventHandler(dataReceiver2);
                if (port2.IsOpen)
                {
                    //Status: Online
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbPort3_SelectedIndexChanged(object sender, EventArgs e)
        {
            port3 = new SerialPort(cbPort3.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            try
            {
                port3.Open();
                port3.DataReceived += new SerialDataReceivedEventHandler(dataReceiver3);
                if (port3.IsOpen)
                {
                    //Status: Online
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void cbPort4_SelectedIndexChanged(object sender, EventArgs e)
        {
            port4 = new SerialPort(cbPort4.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            try
            {
                port4.Open();
                port4.DataReceived += new SerialDataReceivedEventHandler(dataReceiver4);
                if (port4.IsOpen)
                {
                    //Status: Online
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        //Data Receiver and Output
        private void dataReceiver1(object sender, SerialDataReceivedEventArgs e)    //Receiver Inner Box A
        {
            try
            {
                string input_value = port1.ReadExisting();
                Thread.Sleep(60);
                Invoke((MethodInvoker)delegate { tbInnerBoxA.Text = input_value; lbLog.Items.Add(input_value); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
               inner_a_master = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                if (input_value.Equals(inner_a_master))
                {
                    inner_count++;
                    if (inner_count >= innerbox_max) {inner_count = innerbox_max; } //Alarm
                    qty -= 1;
                    if (qty < 0) { qty = 0; }
                    Invoke((MethodInvoker)delegate { lRemainingInner.Text = "Remaining: " + qty.ToString(); });
                    Invoke((MethodInvoker)delegate { lNeedInner.Text = "Need: " + inner_count + " / " + innerbox_max; });
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
                Thread.Sleep(60);
                Invoke((MethodInvoker)delegate { tbInnerBoxB.Text = input_value; lbLog.Items.Add(input_value); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                inner_b_master = WindowsForms_packing_line.Properties.Settings.Default.InnerAMaster;
                if (input_value.Equals(inner_b_master))
                {
                    inner_count++;
                    if ( inner_count >= innerbox_max ) { inner_count = innerbox_max; }  //Alarm
                    qty -= 1;
                    if (qty < 0) { qty = 0; }
                    Invoke((MethodInvoker)delegate { lRemainingInner.Text = "Remaining: " + qty.ToString(); });
                    Invoke((MethodInvoker)delegate { lNeedInner.Text = "Need: " + inner_count + " / " + innerbox_max; });
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
                Thread.Sleep(60);
                Invoke((MethodInvoker)delegate { tbCartonBox.Text = input_value; lbLog.Items.Add(input_value); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                carton_master = WindowsForms_packing_line.Properties.Settings.Default.CartonMaster;
                if (input_value.Equals(carton_master))
                {
                    carton_count++;
                    if (carton_count >= cartonbox_max) { carton_count = cartonbox_max; }
                    cartonbox_rem -= 1;
                    if (cartonbox_rem < 0) { cartonbox_rem = 0; }
                    Invoke((MethodInvoker)delegate { lRemainingCarton.Text = "Remaining: " + cartonbox_rem.ToString(); });
                    Invoke((MethodInvoker)delegate { lNeedCarton.Text = "Need: " + carton_count + " / " + cartonbox_max; });
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
                Thread.Sleep(60);
                Invoke((MethodInvoker)delegate { tbExportBox.Text = input_value; lbLog.Items.Add(input_value); lbLog.SelectedIndex = lbLog.Items.Count - 1; lbLog.SelectedIndex = -1; });
                export_master = WindowsForms_packing_line.Properties.Settings.Default.ExportMaster;
                if (input_value.Equals(export_master))
                {
                    //Reset Kanban
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Button Save Ports
        private void btnSavePorts_Click(object sender, EventArgs e)
        {

        }
        //Start Button
        private void btnStart_Click(object sender, EventArgs e)
        {
            queryQTY();
            queryInnerA();
            queryInnerB();
            queryCarton();
            queryExport();
        }
        //SQL Connect, Get
        public void login()
        {

        }
        public void queryQTY()
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
                    tbModel.Text = reader.GetString(2);
                    tbQTY.Text = reader.GetString(9);
                    qty = reader.GetInt32("Qty");
                    innerbox_max = reader.GetInt32("InnerMax");
                    cartonbox_max = reader.GetInt32("CartonMax");
                    cartonbox_rem  = qty / innerbox_max;
                    lRemainingInner.Text = "Remaining: " + qty.ToString();  //Remaining:
                    lRemainingCarton.Text = "Remaining: " + cartonbox_rem.ToString();    //Remaining:
                    lNeedInner.Text = "Need: " + inner_count + " / " + innerbox_max; //Need:
                    lNeedCarton.Text = "Need: " + carton_count + " / " + cartonbox_max;   //Need:
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
}