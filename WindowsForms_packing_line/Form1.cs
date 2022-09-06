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

namespace WindowsForms_packing_line
{
    public partial class Form1 : Form
    {
        SerialPort port1;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cbPorts.Items.AddRange(ports);
            cbPorts.SelectedIndex = 0;
            port1 = new SerialPort(cbPorts.Text, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            try
            {
                port1.Open();
                if (port1.IsOpen)
                {
                    port1.DataReceived += new SerialDataReceivedEventHandler(dataReceiver);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Data Receiver
        private void dataReceiver(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string input_value = port1.ReadExisting();
                Invoke((MethodInvoker)delegate { textBox1.Text = input_value; });
                Thread.Sleep(200);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
