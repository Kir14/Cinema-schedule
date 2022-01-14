using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KuHo_HO_He_TEATP
{
    public partial class Admin : Form
    {
        OdbcConnection conn;
        DataTable dt;
        OdbcDataAdapter oda;
        string connectingString;

        DataGridViewComboBoxCell dcombo;
        List<string> deleteFoto;

        public Admin()
        {
            InitializeComponent();

            dcombo = new DataGridViewComboBoxCell();
            dcombo.Items.Add("10.00-12.00");
            dcombo.Items.Add("13.00-15.00");
            dcombo.Items.Add("16.00-18.00");

            deleteFoto = new List<string>();

            GetData();
        }

        private void GetData()
        {
            dataGridView.Rows.Clear();
            dt = new DataTable();

            dt.PrimaryKey = new DataColumn[] { dt.Columns["id"] };
            string folder = Environment.CurrentDirectory;
            string file = "БД.txt";
            connectingString = @"Driver={Microsoft Text Driver (*.txt; *.csv)};Dbq=" + folder + ";" +
                "Extensions=asc,csv,tab,txt;Persist Security Info=False";
            conn = new OdbcConnection(connectingString);
            conn.Open();
            oda = new OdbcDataAdapter();

            //string myUpdateQuery = "UPDATE [БД.txt] SET Название=?, Дата=?, Время=?, Зал=?, Цена=?, Постер=?" +
            //    " Where id=?";
            //OdbcCommand myUpdateCommand = new OdbcCommand(myUpdateQuery);
            //myUpdateCommand.Connection = new OdbcConnection(connectingString);

            OdbcCommand mySelectCommand = new OdbcCommand("Select * from [БД.txt]");
            mySelectCommand.Connection = new OdbcConnection(connectingString);


            string myInsertQuery = "Insert into [БД.txt] VALUES (?,?,?,?,?,?,?)";

            OdbcCommand myInsertCommand = new OdbcCommand(myInsertQuery);
            myInsertCommand.Connection = new OdbcConnection(connectingString);

            oda.SelectCommand = mySelectCommand;
            //oda.UpdateCommand = myUpdateCommand;
            //oda.InsertCommand = myInsertCommand;
            oda.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    Image image = new Bitmap(Environment.CurrentDirectory + row["Постер"]);


                    //Заносим данные в таблицу
                    int index = dataGridView.Rows.Add(row["id"], row["Название"], String.Format("{0:dd/MM/yyyy}", row["Дата"]), row["Время"], row["Зал"], Double.Parse(row["Цена"].ToString()), new Bitmap(image, 25, 50));

                    string[] mas = row["Жанр"].ToString().Split(' ');
                    foreach (string el in mas)
                    {
                        if (el == "Боевик")
                        {
                            dataGridView.Rows[index].Cells["Боевик"].Value = true;
                        }
                        if (el == "Фантастика")
                        {
                            dataGridView.Rows[index].Cells["Фантастика"].Value = true;
                        }
                        if (el == "Триллер")
                        {
                            dataGridView.Rows[index].Cells["Триллер"].Value = true;
                        }
                    }

                    dataGridView.Rows[index].Tag = row["Постер"];

                    image.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            dataGridView.Columns[0].ReadOnly = true;
            conn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //Save();

                //Массив для хранения данных из таблицы
                List<string> array = new List<string>();
                array.Add("id,Название,Дата,Время,Зал,Цена,Постер,Жанр");
                string s1;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    //Проверяем был ли добавлен постер или он уже был в системе
                    if (row.Tag.ToString()[0] == '\\')//Постер уже был в системе
                    {
                        s1 = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + ","
                            + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + ","
                            + row.Cells[4].Value.ToString() + ","
                            + Double.Parse(row.Cells[5].Value.ToString()).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ","
                            + row.Tag.ToString() + ",";
                    }
                    else//Новый постер
                    {
                        //Копируем изображение в папку
                        System.IO.File.Copy(row.Tag.ToString(), Environment.CurrentDirectory + "\\Foto\\"
                            + row.Cells[0].Value.ToString() + ".jpg", true);

                        //Формируем строку
                        s1 = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + ","
                            + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + ","
                            + row.Cells[4].Value.ToString() + ","
                            + Double.Parse(row.Cells[5].Value.ToString()).ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US")) + ","
                            + "\\Foto\\" + row.Cells[0].Value.ToString() + ".jpg,";

                    }
                    if (Convert.ToBoolean(row.Cells["Боевик"].EditedFormattedValue))
                    {
                        s1 += "Боевик ";
                    }
                    if (Convert.ToBoolean(row.Cells["Фантастика"].EditedFormattedValue))
                    {
                        s1 += "Фантастика ";
                    }
                    if (Convert.ToBoolean(row.Cells["Триллер"].EditedFormattedValue))
                    {
                        s1 += "Триллер ";
                    }
                    array.Add(s1);
                }
                //Пересоздаем файл и записываем данные из таблицы
                FileStream ous = File.Create("БД.txt");
                StreamWriter sw = new StreamWriter(ous, Encoding.Default);
                foreach (string el in array)
                {
                    sw.WriteLine(el);
                }
                sw.Flush();
                sw.Close();

                foreach (string el in deleteFoto)
                {
                    System.IO.File.Delete(Environment.CurrentDirectory + "\\Foto\\" + el + ".jpg");
                }

                GetData();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnFoto_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentCell.ColumnIndex == 6)
            {
                OpenFileDialog OPF = new OpenFileDialog();
                OPF.Filter = "Файлы jpg|*.jpg";
                if (OPF.ShowDialog() == DialogResult.OK)
                {
                    dataGridView.Rows[dataGridView.CurrentRow.Index].Tag = OPF.FileName.ToString();
                    Image image = new Bitmap(OPF.FileName.ToString());
                    dataGridView[6, dataGridView.CurrentRow.Index].Value = new Bitmap(image, 25, 50);

                    image.Dispose();

                    ////Проверяем есть ли название в строке, где мы добавляем постер
                    //if (dataGridView[1, dataGridView.CurrentRow.Index].Value == null)
                    //{
                    //    dataGridView.Rows[dataGridView.CurrentRow.Index].Tag = "\\Foto\\" + OPF.FileName.Remove(0, OPF.FileName.LastIndexOf('\\') + 1);

                    //    Image image = new Bitmap(OPF.FileName.ToString());
                    //    dataGridView[6, dataGridView.CurrentRow.Index].Value = new Bitmap(image, 25, 50);

                    //    image.Dispose();
                    //}
                    //else
                    //{
                    //    //Для каждого фильма с таким же именем меняем постер
                    //    foreach (DataGridViewRow row in dataGridView.Rows)
                    //    {
                    //        try
                    //        {
                    //            //Проверка имен
                    //            if (row.Cells[1].Value != null &&
                    //                dataGridView[1, dataGridView.CurrentRow.Index].Value.ToString() == row.Cells[1].Value.ToString())
                    //            {
                    //                row.Tag = OPF.FileName.ToString();

                    //                Image image = new Bitmap(OPF.FileName.ToString());
                    //                row.Cells[6].Value = new Bitmap(image, 25, 50);

                    //                image.Dispose();
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            MessageBox.Show(ex.Message);
                    //        }
                    //    }
                    //}
                }
            }
            else
            {
                MessageBox.Show("Выберите ячейку с постером");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    deleteFoto.Add(row.Cells[0].Value.ToString());
                    dataGridView.Rows.Remove(row);
                }
            }
            else
            {
                MessageBox.Show("Выберите всю строку");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            int i = dataGridView.Rows.GetLastRow(DataGridViewElementStates.Visible);
            int j = dataGridView.Rows.Add();
            dataGridView[0, j].Value = Int32.Parse(dataGridView[0, i].Value.ToString()) + 1;

        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;
            int j = e.ColumnIndex;
            int i = e.RowIndex;
            conn.Open();

            string myUpdateQuery = "UPDATE [БД.txt] SET Название=?, Дата=?, Время=?, Зал=?, Цена=?, Постер=?" +
                " Where id=?";
            OdbcCommand myUpdateCommand = new OdbcCommand(myUpdateQuery);
            myUpdateCommand.Connection = new OdbcConnection(connectingString);
            myUpdateCommand.Parameters.Add("@Название", OdbcType.VarChar).Value = dataGridView[1, i].Value;
            myUpdateCommand.Parameters.Add("@Дата", OdbcType.VarChar).Value = dataGridView[2, i].Value;
            myUpdateCommand.Parameters.Add("@Время", OdbcType.VarChar).Value = dataGridView[3, i].Value;
            myUpdateCommand.Parameters.Add("@Зал", OdbcType.VarChar).Value = dataGridView[4, i].Value;
            myUpdateCommand.Parameters.Add("@Цена", OdbcType.Double).Value = dataGridView[5, i].Value;
            myUpdateCommand.Parameters.Add("@Постер", OdbcType.VarChar).Value = dataGridView.Rows[i].Tag.ToString();
            myUpdateCommand.Parameters.Add("@id", OdbcType.Int).Value = dataGridView[0, i].Value;

            //oda.UpdateCommand.Parameters.AddWithValue("Название", dataGridView[1, i].Value);
            //oda.UpdateCommand.Parameters.AddWithValue("Дата", dataGridView[2, i].Value);
            //oda.UpdateCommand.Parameters.AddWithValue("Время", dataGridView[3, i].Value);
            //oda.UpdateCommand.Parameters.AddWithValue("Зал", dataGridView[4, i].Value);
            //oda.UpdateCommand.Parameters.AddWithValue("Цена", dataGridView[5, i].Value);
            //oda.UpdateCommand.Parameters.AddWithValue("Постер", dataGridView.Rows[i].Tag.ToString());
            //oda.UpdateCommand.Parameters.AddWithValue("id", dataGridView[0, i].Value);

            //oda.InsertCommand.Parameters.AddWithValue("id", dataGridView[0, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Название", dataGridView[1, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Дата", dataGridView[2, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Время", dataGridView[3, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Зал", dataGridView[4, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Цена", dataGridView[5, i].Value);
            //oda.InsertCommand.Parameters.AddWithValue("Постер", dataGridView.Rows[i].Tag.ToString());


            OdbcCommandBuilder builder = new OdbcCommandBuilder(oda);

            //builder.QuotePrefix = builder.QuoteSuffix = "\"";
            oda.UpdateCommand = builder.GetUpdateCommand();
            //oda.UpdateCommand = myUpdateCommand;


            dt.Rows[i][j] = dataGridView[j, i].Value;
            //oda.UpdateCommand = new OdbcCommandBuilder(oda).GetUpdateCommand();
            Console.WriteLine(oda.Update(dt));
            Console.WriteLine(oda.Fill(dt));

            dt.AcceptChanges();
            conn.Close();
            //GetData();
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }
    }
}
