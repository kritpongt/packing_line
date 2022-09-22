using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms_packing_line
{
    public partial class Authentication : Form
    {
        string connectStr = Form1.connectStr;
        SerialPort portRFID = Form1.portRFID;
        public static bool alarm_turn_off = false;
        public Authentication()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.ActiveControl = tbAlarm;
        }
        private void tbAlarm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string TABLE = "account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE operatorID = '" + tbAlarm.Text + "';";
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
                        if (reader.GetString("position").Equals("Administrator") || reader.GetString("position").Equals("Supervisor"))
                        {
                            this.Close();
                            alarm_turn_off = true;
                        }
                    }
                    tbAlarm.SelectAll();
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
            string TABLE = "account";
            string queryList = "SELECT * FROM " + TABLE + " WHERE operatorID = '" + tbAlarm.Text + "';";
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
                    if (reader.GetString("position").Equals("Administrator") || reader.GetString("pposition").Equals("Supervisor"))
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
                tbAlarm.Focus();
            }
        }//Waittest
        private void btnRFID_Click(object sender, EventArgs e)
        {
            try
            {
                portRFID.Open();
                portRFID.DataReceived += new SerialDataReceivedEventHandler(dataReceiverRFIDAlarm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataReceiverRFIDAlarm(object sender, SerialDataReceivedEventArgs e)
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
                MessageBox.Show(s_buffer); //test
                //code below
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//Waittest
    }
}
