using ExcelLibrary.SpreadSheet;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace WindowsForms_packing_line
{
    public partial class Countperday : Form
    {
        string connectStr = Form1.connectStr;
        public Countperday()
        {
            InitializeComponent();
            calPerday(dbDistinctKanban());
        }
        public void dbCountperday()
        {
            lvCountperday.Items.Clear();
            string TABLE = "test_countperday";
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
                    ListViewItem list = new ListViewItem(dr["Kanban"].ToString());
                    list.SubItems.Add(dr["Count"].ToString());
                    lvCountperday.Items.Add(list);
                }
                lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
        public int dbCountBox(string kanban, string count)
        {
            int countbox = 0;
            string queryList = "SELECT COUNT(kanban) FROM test_countperday WHERE Kanban = '" + kanban + "' AND Count = '" + count + "';";
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
                    countbox = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return countbox;
        }
        public string[] dbDistinctKanban()
        {
            int index_lenght = 0;
            string queryList = "SELECT COUNT(DISTINCT Kanban) FROM test_countperday;";
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
                    index_lenght = reader.GetInt32(0);
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
            int index = 0;
            string[] kanban_no = new string[index_lenght];
            string queryList2 = "SELECT DISTINCT Kanban FROM test_countperday;";
            MySqlConnection dbconnect2 = new MySqlConnection(connectStr);
            MySqlCommand dbcommand2 = new MySqlCommand(queryList2, dbconnect2);
            MySqlDataReader reader2;
            dbcommand.CommandTimeout = 60;
            try
            {
                dbconnect2.Open();
                reader2 = dbcommand2.ExecuteReader();
                while (reader2.Read())
                {
                    kanban_no[index] = reader2.GetString("Kanban");
                    index++;
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
            return kanban_no;
        }
        public void calPerday(string[] kanban_perday)
        {
            int countInnerBox = 0, countCartonBox = 0, countExportBox = 0;
            for (int i = 0; i < kanban_perday.Length; i++)
            {
                countInnerBox += dbCountBox(kanban_perday[i], "Inner Box");
                countCartonBox += dbCountBox(kanban_perday[i], "Carton Box");
                countExportBox += dbCountBox(kanban_perday[i], "Export Box");
                string[] list = { (i+1).ToString(), kanban_perday[i], countInnerBox.ToString(), countCartonBox.ToString(), countExportBox.ToString() };
                ListViewItem item = new ListViewItem(list);
                lvCountperday.Items.Add(item);
            }
            lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (lvCountperday.Items.Count == 0)
            {
                MessageBox.Show("No data to Export!");
            }
            else
            {
                exportExcel();
            }
        }
        public void exportExcel()
        {
            string file_name = "";
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = @"C:/";
            save.Title = "Export Excel Files";
            save.CheckFileExists = false;
            save.CheckPathExists = true;
            save.DefaultExt = "xls";
            save.Filter = "Excel files (*.xls)|*.xls";
            save.FilterIndex = 1;
            save.RestoreDirectory = true;
            if(save.ShowDialog() == DialogResult.OK)
            {
                file_name = save.FileName;
                try
                {
                    ExcelLibrary.SpreadSheet.Workbook workbook = new ExcelLibrary.SpreadSheet.Workbook();
                    ExcelLibrary.SpreadSheet.Worksheet worksheet = new ExcelLibrary.SpreadSheet.Worksheet("Counter/Day");

                    MessageBox.Show("Excel Create Completed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
