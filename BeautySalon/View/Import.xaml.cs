using BeautySalon.viewBase;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeautySalon.View
{
    /// <summary>
    /// Логика взаимодействия для Import.xaml
    /// </summary>
    public partial class Import : UserControl
    {
        public Import()
        {
            InitializeComponent();
        }
        string fileName;
        private object tableName;

        public object ComboBox1 { get; private set; }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы csv| *.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                FileName.Text = fileInfo.ToString();
                fileName = fileInfo.ToString();
            }        
        }
        private void FileBtn_Click(object sender, RoutedEventArgs e)
        {
            string[] readText = File.ReadAllLines(fileName, Encoding.GetEncoding(1251));
            string[] titleField = readText[0].Split(';').Select(field => field.Trim().Trim('"')).ToArray();
            string tableName = Combobox1.Text;
            string[] valField;
            string que = string.Empty;
            int count;           

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                string[] dbHeaders = GetDataBaseHeader(con, tableName);
                con.Close();

                if (titleField.SequenceEqual(dbHeaders))
                {
                    con.Open();
                    MySqlCommand cmdClearTable = new MySqlCommand($"DELETE FROM `{tableName}`;", con);
                    cmdClearTable.ExecuteNonQuery();

                    foreach (string str in readText.Skip(1).ToArray())
                    {
                        valField = str.Split(';').Select(field => field.Trim().Trim('"')).ToArray();
                        switch (tableName)
                        {
                            case "Certificates":
                                MySqlCommand cmdCheckTableServices = new MySqlCommand("SELECT COUNT(*) FROM Services;", con);
                                MySqlDataAdapter da = new MySqlDataAdapter(cmdCheckTableServices);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                if (int.Parse(dt.Rows[0].ItemArray[0].ToString()) > 0)
                                {
                                    que = $@"Insert Into `Certificates`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}')";
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Сначала заполните таблицу Services");
                                    return;
                                }
                            case "Clients":
                                que = $@"Insert Into `Clients`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}')";
                                break;
                            case "Cosmetic_Products":
                                que = $@"Insert Into `Cosmetic_Products`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}','{valField[6]}')";
                                break;
                            case "Employee_Schedules":
                                MySqlCommand cmdCheckTableEmployees = new MySqlCommand("SELECT COUNT(*) FROM Employees;", con);
                                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(cmdCheckTableEmployees);
                                MySqlDataAdapter da1 = mySqlDataAdapter;
                                DataTable dt1 = new DataTable();
                                da1.Fill(dt1);
                                if (int.Parse(dt1.Rows[0].ItemArray[0].ToString()) > 0)
                                {
                                que = $@"Insert Into `Employee_Schedules`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}')";
                                break;
                                }
                                else
                                {
                                    MessageBox.Show("Сначала заполните таблицу Employees");
                                    return;
                                }
                            case "Employees":
                                que = $@"Insert Into `Employees`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}','{valField[6]}')";
                                break;
                            case "Materials":
                                que = $@"Insert Into `Materials`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}')";
                                break;
                            case "Orders_Cosmetic_Products":
                                MySqlCommand cmdCheckTableClients = new MySqlCommand("SELECT COUNT(*) FROM Clients;", con);
                                MySqlDataAdapter mySqlDataClients = new MySqlDataAdapter(cmdCheckTableClients);
                                DataTable dtClients = new DataTable();
                                mySqlDataClients.Fill(dtClients);

                                MySqlCommand cmdCheckTableCosmetics = new MySqlCommand("SELECT COUNT(*) FROM Cosmetic_products;", con);
                                MySqlDataAdapter mySqlDataCosmetics = new MySqlDataAdapter(cmdCheckTableCosmetics);
                                DataTable dtCosmetics = new DataTable();
                                mySqlDataCosmetics.Fill(dtCosmetics);

                                if (int.Parse(dtClients.Rows[0].ItemArray[0].ToString()) > 0 && int.Parse(dtCosmetics.Rows[0].ItemArray[0].ToString()) > 0)
                                {
                                    que = $@"Insert Into Orders_Cosmetic_Products({String.Join(",", titleField)}) VALUES(
            '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}')";
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Сначала заполните таблицы Clients и Cosmetic_products");
                                    return;
                                }
                            case "Recording":
                                MySqlCommand cmdCheckTableClient = new MySqlCommand("SELECT COUNT(*) FROM Clients;", con);
                                MySqlDataAdapter mySqlDataClient = new MySqlDataAdapter(cmdCheckTableClient);
                                DataTable dtClient = new DataTable();
                                mySqlDataClient.Fill(dtClient);

                                MySqlCommand cmdCheckTableService = new MySqlCommand("SELECT COUNT(*) FROM Services;", con);
                                MySqlDataAdapter mySqlDataService = new MySqlDataAdapter(cmdCheckTableService);
                                DataTable dtServices = new DataTable();
                                mySqlDataService.Fill(dtServices);

                                if (int.Parse(dtClient.Rows[0].ItemArray[0].ToString()) > 0 && int.Parse(dtServices.Rows[0].ItemArray[0].ToString()) > 0)
                                {
                                    que = $@"Insert Into Recording({String.Join(",", titleField)}) VALUES(
        '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}')";
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Сначала заполните таблицы Clients и Services");
                                    return;
                                }
                            case "Service_Materials":
                                // Проверка на наличие записей в таблице Services
                                MySqlCommand cmdCheckTableServicess = new MySqlCommand("SELECT COUNT(*) FROM Services;", con);
                                MySqlDataAdapter mySqlDataServicess = new MySqlDataAdapter(cmdCheckTableServicess);
                                DataTable dtServicess = new DataTable();
                                mySqlDataServicess.Fill(dtServicess);
                                // Проверка на наличие записей в таблице Materials
                                MySqlCommand cmdCheckTableMaterials = new MySqlCommand("SELECT COUNT(*) FROM Materials;", con);
                                MySqlDataAdapter mySqlDataMaterials = new MySqlDataAdapter(cmdCheckTableMaterials);
                                DataTable dtMaterials = new DataTable();
                                mySqlDataMaterials.Fill(dtMaterials);
                                // Условие для вставки в таблицу Service_Materials
                                if (int.Parse(dtServicess.Rows[0].ItemArray[0].ToString()) > 0 && int.Parse(dtMaterials.Rows[0].ItemArray[0].ToString()) > 0)
                                {
                                    que = $@"Insert Into Service_Materials({String.Join(",", titleField)}) VALUES(
        '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}')";
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Сначала заполните таблицы Services и Materials");
                                    return;
                                }
                            case "Services":
                                que = $@"Insert Into `Services`({String.Join(",", titleField)}) VALUES(
                                '{valField[0]}','{valField[1]}','{valField[2]}','{valField[3]}','{valField[4]}')";
                                break;     
                        }
                        using (MySqlCommand cmd = new MySqlCommand(que, con))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    MessageBox.Show("Данные успешно импортированы","Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Combobox1.SelectedItem = null;
                    FileBtn.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Ошибка импортирования данных", "Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }
            private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                FileBtn.IsEnabled = false;
                try
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand($@"SHOW TABLES", con))
                    {
                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            Combobox1.Items.Add(dr.GetValue(0));
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка с подключением", "Ошикба", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                }               
        }
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Combobox1.Template.FindName("textBlock", Combobox1);
            if (Combobox1.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
                FileBtn.IsEnabled = true;
            }
        }  
        static string[] GetDataBaseHeader(MySqlConnection con, string tableName)
        {
            using (MySqlCommand cmd = new MySqlCommand($"Select * From {tableName} Limit 0", con))
            {
                MySqlDataReader dr = cmd.ExecuteReader();
                var columnNames = Enumerable.Range(0, dr.FieldCount)
                .Select(dr.GetName)
                .ToArray();
                return columnNames;
            }
        }
    }
}