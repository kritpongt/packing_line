using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms_packing_line
{
    public partial class Authentication : Form
    {
        string connectStr = "server=" + WindowsForms_packing_line.Properties.Settings.Default.dbIPServer + ";port=3306;Database=packing_line_element;uid=root;pwd=;SslMode=none;";
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
                string TABLE = "test_account";
                string queryList = "SELECT * FROM " + TABLE + " WHERE OperatorID = '" + tbAlarm.Text + "';";
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
                        this.Close();
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
}
