using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using BeautySalon.Forms;
using System.Text.RegularExpressions;
using System.IO;
using OfficeOpenXml;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Materials.xaml
    /// </summary>
    public partial class Materials : UserControl
    {
        public string query = "SELECT material_id, material_name AS 'Название материала', " +
                "type AS 'Тип', description AS 'Описание', quantity_in_stock AS 'Остаток на складе', price AS 'Стоимость' FROM Materials where isDelete ='0'";
        public Materials()
        {
            InitializeComponent();
        }

        private DataTable dt;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyData.role == "Директор")
            {
                EBtn.Content = "Отчёты";
            }
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                    string query = @"SELECT material_id, material_name AS 'Название материала', 
                                     type AS 'Тип', description AS 'Описание', quantity_in_stock AS 'Остаток на складе', price AS 'Стоимость' FROM Materials where isDelete ='0'";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);

                        dataGridView.ItemsSource = dt.DefaultView;
                        dataGridView.Columns[0].Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteringAndSorting();
        }
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            filteringAndSorting();
        }
        private void UpdateDataGridView(string query)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(SqlConnection.connectionString))
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                connection.Open();
                try
                {
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при извлечении данных: {ex.Message}");
                }
            }
            dataGridView.ItemsSource = dataTable.DefaultView;
            dataGridView.Columns[0].Visibility = Visibility.Collapsed;
        }
        private void filteringAndSorting()
        {
            query = "SELECT material_id, material_name AS 'Название материала', " +
                  "type AS 'Тип', description AS 'Описание', quantity_in_stock AS 'Остаток на складе', price AS 'Стоимость' FROM Materials where isDelete ='0'";
            string sortOrder = "";

            if (ComboBox1.SelectedItem != null)
            {
                string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedSortValue)
                {
                    case "По Возврастанию":
                        sortOrder += "ORDER BY material_name ASC";
                        break;
                    case "По Убыванию":
                        sortOrder += "ORDER BY material_name DESC";
                        break;
                }
            }
            if (ComboBox2.SelectedItem != null)
            {
                string selectedTypeValue = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();
                query += $" AND type = '{selectedTypeValue}'";
            }

            string filterText = searchBox.Text.Trim();
            if (!string.IsNullOrEmpty(filterText))
            {
                if (ComboBox2.SelectedItem != null)
                {
                    query += $" AND material_name LIKE '%{filterText}%'";
                }
                else
                {
                    query += $" AND material_name LIKE '%{filterText}%'";
                }
            }
            if (sortOrder != null)
            {
                query += " " + sortOrder;
            }

            UpdateDataGridView(query);
        }
        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            {
                filteringAndSorting();
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            MaterialsAdd materialsAdd = new MaterialsAdd();
            MyData.idleTimer.Stop();
            materialsAdd.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateDataGridView(query);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();


                var result = MessageBox.Show("Вы действительно хотите удалить данный материал?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"Update Materials set isDelete='1'  where material_id ='{MyData.id}'", con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Материал удалён");
                    }
                    UpdateDataGridView(query);
                }
                EditBtn.IsEnabled = false;
                DellBtn.IsEnabled = false;
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            MaterialEdit materialEdit = new MaterialEdit();
            MyData.idleTimer.Stop();
            materialEdit.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            this.Opacity = 1;
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
            else
            {
                MyData.id = null;
            }
        }

        private void EBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.License.SetNonCommercialOrganization("<Your Noncommercial Organization>");
            string query = "SELECT material_id As '№', material_name AS 'Название материала', " +
                           "type AS 'Тип', quantity_in_stock AS 'Остаток на складе', price AS 'Стоимость' " +
                           "FROM Materials where isDelete ='0' and quantity_in_stock < 5";

            using (MySqlConnection connection = new MySqlConnection(SqlConnection.connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                    return;
                }
                // Путь к папке "Отчёты"
                string reportsDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "Отчёты");

                // Создание папки, если она не существует
                if (!Directory.Exists(reportsDirectory))
                {
                    Directory.CreateDirectory(reportsDirectory);
                }
                // Проверка роли пользователя
                if (MyData.role == "Директор")
                {
                    // Открытие папки "Отчёты"
                    System.Diagnostics.Process.Start("explorer.exe", reportsDirectory);
                    return;
                }
                // Создание Excel-файла
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Отчет");
                    string reportTitle = $"Отчет от {DateTime.Now.ToString("dd.MM.yyyy")}, продукты необходимые для закупки";
                    worksheet.Cells[1, 1].Value = reportTitle;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[2, i + 1].Value = dataTable.Columns[i].ColumnName;
                        worksheet.Cells[2, i + 1].Style.Font.Size = 14;
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 3, j + 1].Value = dataTable.Rows[i][j];
                            worksheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                        }
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Формирование полного пути к файлу
                    string filePath = System.IO.Path.Combine(reportsDirectory, $"Отчет{DateTime.Now.ToString("dd.MM.yyyy")}.xlsx");
                    FileInfo fileInfo = new FileInfo(filePath);
                    excelPackage.SaveAs(fileInfo);

                    MessageBox.Show("Отчет сохранен: " + filePath);
                }
            }
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            searchBox.Text = string.Empty;
            query = "SELECT material_id, material_name AS 'Название материала', " +
                 "type AS 'Тип', description AS 'Описание', quantity_in_stock AS 'Остаток на складе', price AS 'Стоимость' FROM Materials where isDelete ='0'";
            UpdateDataGridView(query);
        }

        private void searchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
    }
}

