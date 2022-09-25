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
        struct TotalPerDay
        {
            public int total_kanban;
            public int total_inner_box_a;
            public int total_inner_box_b;
            public int total_carton_box;
            public int total_export_box;
            public int total_inner_box;
        }
        TotalPerDay totalperday;
        public Countperday()
        {
            InitializeComponent();
            calPerday(dbDistinctKanban());
        }
        public void dbCountperday()
        {
            lvCountperday.Items.Clear();
            string TABLE = "countperday";
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
        }//not connect
        public int dbCountBox(string kanban, string count)
        {
            int countbox = 0;
            string queryList = "SELECT COUNT(kanban) FROM countperday WHERE kanban = '" + kanban + "' AND countFrom = '" + count + "';";
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
                MessageBox.Show(ex.Message, "dbCountBox", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return countbox;
        }
        public string[] dbDistinctKanban()
        {
            int index_lenght = 0;
            string queryList = "SELECT COUNT(DISTINCT kanban) FROM countperday;";
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
                MessageBox.Show(ex.Message, "dbDistinct", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbconnect.Close();
            }
            int index = 0;
            string[] kanban_no = new string[index_lenght];
            string queryList2 = "SELECT DISTINCT kanban FROM countperday;";
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
                    kanban_no[index] = reader2.GetString("kanban");
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
            int countInnerBoxA = 0, countInnerBoxB = 0, countCartonBox = 0, countExportBox = 0, total = 0;
            for (int i = 0; i < kanban_perday.Length; i++)
            {
                int dbInnerBoxA = dbCountBox(kanban_perday[i], "Inner Box A");
                int dbInnerBoxB = dbCountBox(kanban_perday[i], "Inner Box B");
                int dbCartonBox = dbCountBox(kanban_perday[i], "Carton Box");
                int dbExportBox = dbCountBox(kanban_perday[i], "Export Box");
                countInnerBoxA += dbInnerBoxA;
                countInnerBoxB += dbInnerBoxB;
                countCartonBox += dbCartonBox;
                countExportBox += dbExportBox;
                total = countInnerBoxA + countInnerBoxB;
                string[] list = { (i+1).ToString(), kanban_perday[i], dbInnerBoxA.ToString(), dbInnerBoxB.ToString(), dbCartonBox.ToString(), dbExportBox.ToString() };
                ListViewItem item = new ListViewItem(list);
                lvCountperday.Items.Add(item);
            }
            lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lvCountperday.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            totalperday.total_kanban = kanban_perday.Length;
            totalperday.total_inner_box_a = countInnerBoxA;
            totalperday.total_inner_box_b = countInnerBoxB;
            totalperday.total_carton_box = countCartonBox;
            totalperday.total_export_box = countExportBox;
            totalperday.total_inner_box = total;
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
                    worksheet.Cells[0, 0] = new Cell("Date: ");
                    worksheet.Cells[1, 0] = new Cell("Total K/B");
                    worksheet.Cells[2, 0] = new Cell("Total Inner Box A");
                    worksheet.Cells[3, 0] = new Cell("Total Inner Box B");
                    worksheet.Cells[4, 0] = new Cell("Total Carton Box");
                    worksheet.Cells[5, 0] = new Cell("Total Export Box");
                    worksheet.Cells[6, 0] = new Cell("Total");
                    worksheet.Cells[1, 1] = new Cell(totalperday.total_kanban);
                    worksheet.Cells[2, 1] = new Cell(totalperday.total_inner_box_a);
                    worksheet.Cells[3, 1] = new Cell(totalperday.total_inner_box_b);
                    worksheet.Cells[4, 1] = new Cell(totalperday.total_carton_box);
                    worksheet.Cells[5, 1] = new Cell(totalperday.total_export_box);
                    worksheet.Cells[6, 1] = new Cell(totalperday.total_inner_box);
                    int indexStart = 8;
                    int startTable = indexStart + 1;
                    worksheet.Cells[indexStart, 0] = new Cell("No.");
                    worksheet.Cells[indexStart, 1] = new Cell("Kanban");
                    worksheet.Cells[indexStart, 2] = new Cell("Inner Box A");
                    worksheet.Cells[indexStart, 3] = new Cell("Inner Box B");
                    worksheet.Cells[indexStart, 4] = new Cell("Carton Box");
                    worksheet.Cells[indexStart, 5] = new Cell("Export Box");
                    for (int i = 0; i < lvCountperday.Items.Count; i++)
                    {
                        worksheet.Cells[i + startTable, 0] = new Cell(lvCountperday.Items[i].SubItems[0].Text);
                        worksheet.Cells[i + startTable, 1] = new Cell(lvCountperday.Items[i].SubItems[1].Text);
                        worksheet.Cells[i + startTable, 2] = new Cell(lvCountperday.Items[i].SubItems[2].Text);
                        worksheet.Cells[i + startTable, 3] = new Cell(lvCountperday.Items[i].SubItems[3].Text);
                        worksheet.Cells[i + startTable, 4] = new Cell(lvCountperday.Items[i].SubItems[4].Text);
                        worksheet.Cells[i + startTable, 5] = new Cell(lvCountperday.Items[i].SubItems[5].Text);
                    }
                    workbook.Worksheets.Add(worksheet);
                    workbook.Save(file_name);
                    MessageBox.Show("Excel Create Completed", "Export to Excel");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnResetCountperday_Click(object sender, EventArgs e)
        {
            DialogResult dialog_result = MessageBox.Show("Are you sure to Reset Database counter per day?(จะลบตางรางด้านซ้ายมือด้วย)", "Reset Counter", MessageBoxButtons.YesNo);
            if (dialog_result == DialogResult.Yes)
            {
                string queryList = "DELETE FROM `countperday`;";
                MySqlConnection dbconnect = new MySqlConnection(connectStr);
                MySqlCommand dbcommand = new MySqlCommand(queryList, dbconnect);
                MySqlDataReader reader;
                dbcommand.CommandTimeout = 60;
                try
                {
                    dbconnect.Open();
                    reader = dbcommand.ExecuteReader();
                    MessageBox.Show("Reset Counter is completed.", "Database Countperday");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    dbconnect.Close();
                }
                //reset auto increment
                //string queryList2 = "AFTER TABLE test_countperday AUTO_INCREMENT = value;";
                //MySqlCommand dbcommand2 = new MySqlCommand(queryList2, dbconnect);
                //MySqlDataReader reader2;
                //dbcommand.CommandTimeout = 60;
                //try
                //{
                //    dbconnect.Open();
                //    reader2 = dbcommand2.ExecuteReader();
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                //finally
                //{
                //    dbconnect.Close();
                //}
                lvCountperday.Items.Clear();
            }
        }
    }
}
