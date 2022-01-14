using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KuHo_HO_He_TEATP
{
    public partial class Form1 : Form
    {
        OdbcConnection conn;
        DataTable dt;
        OdbcDataAdapter oda;

        string connectingString;

        public Form1()
        {
            InitializeComponent();
            Connecting();
        }

        public void Connecting()
        {
            
            //System.Data.Odbc.OdbcConnection conn;
            dt = new DataTable();
            //System.Data.Odbc.OdbcDataAdapter oda;
            string folder = Environment.CurrentDirectory;
            string file = "БД.txt";
            conn = new OdbcConnection(@"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + folder + ";Extensions=asc,csv,tab,txt;Persist Security Info=False");
            oda = new OdbcDataAdapter();

            connectingString = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + folder + ";" +
                "Extensions=asc,csv,tab,txt;Persist Security Info=False";


        }

        public void GetData()
        {
            int count = 0;
            string query = "Select * from [БД.txt] WHERE (Дата=?)";
            
            if(textName.Text!="")
            {
                query += " AND (Название like ?)";
            }
            if (comboBox.Text != "")
            {
                query += " AND (Время=?)";
            }
            if (textOT.Text != "")
            {
                query +=" AND (Цена>=?)";
            }
            if (textDO.Text != "")
            {
                query += " AND (Цена<=?)";
            }
            if(checkBox1.Checked || checkBox2.Checked || checkBox3.Checked)
            {
                
                if (checkBox1.Checked)
                {
                    count++;
                }
                if (checkBox2.Checked)
                {
                    count++;
                }
                if (checkBox3.Checked)
                {
                    count++;
                }
                query += " AND (";
                if (count == 3)
                    query += "Жанр like ? OR Жанр like ? OR Жанр like ?";
                else if (count == 2)
                    query += "Жанр like ? OR Жанр like ?";
                else
                    query += "Жанр like ?";
                query += " )";
            }

            OdbcCommand mySelectCommand = new OdbcCommand(query);
            mySelectCommand.Connection = new OdbcConnection(connectingString);
            //mySelectCommand.Parameters.Add("@Название", OdbcType.VarChar).Value = "%"+textName.Text+"%";
            mySelectCommand.Parameters.Add("@Дата", OdbcType.VarChar).Value = dateTimePicker.Value.Date;


            if (textName.Text != "")
            {
                mySelectCommand.Parameters.Add("@Название", OdbcType.VarChar).Value = "%" + textName.Text + "%";
            }
            if (comboBox.Text != "")
            {
                mySelectCommand.Parameters.Add("@Время", OdbcType.VarChar).Value = comboBox.Text;
            }
            if (textOT.Text != "")
            {
                try
                {
                    mySelectCommand.Parameters.Add("@Цена", OdbcType.Double).Value = Double.Parse(textOT.Text.ToString());
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            if (textDO.Text != "")
            {
                try
                {
                    mySelectCommand.Parameters.Add("@Цена", OdbcType.Double).Value = Double.Parse(textDO.Text.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if(checkBox1.Checked)
            {
                try
                {
                    mySelectCommand.Parameters.Add("@Жанр", OdbcType.VarChar).Value = "%" + checkBox1.Text + "%";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (checkBox2.Checked)
            {
                try
                {
                    mySelectCommand.Parameters.Add("@Жанр", OdbcType.VarChar).Value = "%" + checkBox2.Text + "%";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (checkBox3.Checked)
            {
                try
                {
                    mySelectCommand.Parameters.Add("@Жанр", OdbcType.VarChar).Value = "%" + checkBox3.Text + "%";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            oda.SelectCommand = mySelectCommand;
            dt.Rows.Clear();
            oda.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.Columns["Постер"].Visible = false;
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.Columns["Жанр"].Visible = false;
        }

        private void comboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            GetData();
            comboBox.Text = "";
            comboBox.Items.Clear();

            comboBox.Sorted = true;
            foreach (DataRow row in dt.Rows)
            {
                bool add = true;
                foreach (string item in comboBox.Items)
                {
                    if (item == row["Время"].ToString())
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    comboBox.Items.Add(row["Время"]);
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                try
                {
                    pictureBox.Load(Environment.CurrentDirectory + row.Cells[6].Value.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void textName_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void textOT_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void textDO_TextChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            GetData();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            GetData();
        }
    }
}
