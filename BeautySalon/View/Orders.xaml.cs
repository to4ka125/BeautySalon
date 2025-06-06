using BeautySalon.Forms;
using BeautySalon.viewBase;
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
using System.Reflection;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : UserControl
    {
        private readonly string FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Template", "check.docx");
        public string services_id;
        public string DataTime;
        Microsoft.Office.Interop.Word.Document wordDocument;

        public string query = @" SELECT 
    r.recording_id, 
    r.service_id, 
    CONCAT_WS(' ', c.first_name, c.last_name) AS 'Клиент',
    s.service_name AS 'Услуга',
    e.first_name AS 'Мастер',
    DATE_FORMAT(r.recording_datetime, '%d.%m.%Y %H:%i') AS 'Дата записи', 
    r.status AS 'Статус' 
FROM 
    Recording r
INNER JOIN 
    Clients c ON c.client_id = r.client_id
INNER JOIN 
    Services s ON s.service_id = r.service_id
LEFT JOIN 
    Employees e ON e.employee_id = r.employee_id  
ORDER BY 
  r.recording_datetime desc ";
        public Orders()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            OrdersAdd OrdersAdd = new OrdersAdd();
            MyData.idleTimer.Stop();
            OrdersAdd.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1; button.IsEnabled = false;
            button1.IsEnabled = false;
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UpdateGrid(string query)
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
            dataGridView.Columns[1].Visibility = Visibility.Collapsed;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($"Delete From Recording Ads Where recording_id ='{MyData.orders_id}'", con))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись удалена");
                }
                UpdateGrid(query);
            }
            button.IsEnabled = false;
            button1.IsEnabled = false;
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            query = @" SELECT 
    r.recording_id, 
    r.service_id, 
    CONCAT_WS(' ', c.first_name, c.last_name) AS 'Клиент',
    s.service_name AS 'Услуга',
    e.first_name AS 'Мастер',
    DATE_FORMAT(r.recording_datetime, '%d.%m.%Y %H:%i') AS 'Дата записи', 
    r.status AS 'Статус' 
FROM 
    Recording r
INNER JOIN 
    Clients c ON c.client_id = r.client_id
INNER JOIN 
    Services s ON s.service_id = r.service_id
LEFT JOIN 
    Employees e ON e.employee_id = r.employee_id  
 ";

            if (MyData.role == "Мастер")
            {
                query += $@" where r.employee_id = '{MyData.employee_idLoaded}'";

                string sortOrder = "";
                if (ComboBox1.SelectedItem != null)
                {
                    string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

                    switch (selectedSortValue)
                    {
                        case "На исполнении":
                            sortOrder += @" and status = 'На исполнении' ORDER BY 
  r.recording_datetime desc";
                            break;
                        case "Выполнено":
                            sortOrder += @" and status = 'Выполнено' ORDER BY 
  r.recording_datetime desc";
                            break;
                        case "Отменен":
                            sortOrder += @" and status = 'Отменен' ORDER BY 
  r.recording_datetime desc";
                            break;
                    }

                    if (sortOrder != null)
                    {
                        query += " " + sortOrder;
                    }
                }
            }
            else
            {
                string sortOrder = "";
                if (ComboBox1.SelectedItem != null)
                {
                    string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();

                    switch (selectedSortValue)
                    {
                        case "На исполнении":
                            sortOrder += @" where status = 'На исполнении' ORDER BY 
  r.recording_datetime desc";
                            break;
                        case "Выполнено":
                            sortOrder += @" where status = 'Выполнено' ORDER BY 
  r.recording_datetime desc";
                            break;
                        case "Отменен":
                            sortOrder += @" where status = 'Отменен' ORDER BY 
  r.recording_datetime desc";
                            break;
                    }

                    if (sortOrder != null)
                    {
                        query += " " + sortOrder;
                    }
                }
            }

            UpdateGrid(query);
        }
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.orders_id = selectedRow[0].ToString();
                services_id = selectedRow[1].ToString();
                DataTime = selectedRow[4].ToString();
                button.IsEnabled = true;
                button1.IsEnabled = true;
            }
            else
            {

                MyData.orders_id = null;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            button.IsEnabled = false;
            button1.IsEnabled = false;
         

            switch (MyData.role)
            {

                case "Администратор":
            
                    break;

                case "Мастер":
                    query = @" SELECT 
    r.recording_id, 
    r.service_id, 
    CONCAT_WS(' ', c.first_name, c.last_name) AS 'Клиент',
    s.service_name AS 'Услуга',
    e.first_name AS 'Мастер',
    DATE_FORMAT(r.recording_datetime, '%d.%m.%Y %H:%i') AS 'Дата записи', 
    r.status AS 'Статус' 
FROM 
    Recording r
INNER JOIN 
    Clients c ON c.client_id = r.client_id
INNER JOIN 
    Services s ON s.service_id = r.service_id
LEFT JOIN 
    Employees e ON e.employee_id = r.employee_id  
";
                    AddBtn.Visibility = Visibility.Collapsed;
                    button.Visibility = Visibility.Collapsed;
                    button1.Visibility= Visibility.Collapsed;
                    query += $@" where r.employee_id = '{MyData.employee_idLoaded}' ORDER BY 
  r.recording_datetime desc";

                    break;

                case "Директор":

                    break;

                case "Менеджер":
                    break;
            }

            UpdateGrid(query);
        }

        private void button_Click_2(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                using (MySqlCommand checkCmd = new MySqlCommand(
                $"SELECT status FROM Recording WHERE recording_id='{MyData.orders_id}'", con))
                {
                    string currentStatus = checkCmd.ExecuteScalar()?.ToString();


                    if (currentStatus != "На исполнении")
                    {
                        MessageBox.Show($"Нельзя завершить заказ со статусом: {currentStatus}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                        return;
                    }
                }
                var wordApp = new Microsoft.Office.Interop.Word.Application();
                try
                {
                    wordDocument = wordApp.Documents.Open(FileName);
                }
                catch
                {
                    String path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "check.docx");
                    var wordDocument = wordApp.Documents.Open(FileName);
                }
       

                using (MySqlCommand cmd = new MySqlCommand($@"Update Recording Set status = 'Выполнено' where recording_id='{MyData.orders_id}'", con))
                {
                    if (MessageBox.Show("Изменить статус на выполнено?", "Изменение статуса", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        cmd.ExecuteNonQuery();


                        if (MessageBox.Show("Распечать чек", "Чек", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            using (MySqlCommand cmdCheck = new MySqlCommand($"Select service_name,price From Services Where service_id = '{services_id}'", con))
                            {
                                MySqlDataAdapter da = new MySqlDataAdapter(cmdCheck);

                                DataTable dt = new DataTable();

                                da.Fill(dt);

                                string services_name = dt.Rows[0].ItemArray[0].ToString();
                                string price = dt.Rows[0].ItemArray[1].ToString();
                                string dataTime = DateTime.Now.ToString("dd-MM-yyyy HH.MM");

                                wordApp.Visible = false;

                                try
                                {

                                    ReplaceWordStub("{first_name}", MyData.name, wordDocument);
                                    ReplaceWordStub("{service_name}", services_name, wordDocument);
                                    ReplaceWordStub("{data}", dataTime, wordDocument);
                                    ReplaceWordStub("{price}", price, wordDocument);
                                    ReplaceWordStub("{sum}", price, wordDocument);


                                    wordApp.Visible = true;
                                }
                                catch
                                {
                                    MessageBox.Show("Ошибка создания отчета");
                                }
                            }

                        }
                        MessageBox.Show("Статус заказа изменен");
                        UpdateGrid(query);
                        button.IsEnabled = false;
                        button1.IsEnabled = false;
                    }
                }
            }
        }

        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            ComboBox1.SelectedItem = null;
        query = @" SELECT 
    r.recording_id, 
    r.service_id, 
    CONCAT_WS(' ', c.first_name, c.last_name) AS 'Клиент',
    s.service_name AS 'Услуга',
    e.first_name AS 'Мастер',
    DATE_FORMAT(r.recording_datetime, '%d.%m.%Y %H:%i') AS 'Дата записи', 
    r.status AS 'Статус' 
FROM 
    Recording r
INNER JOIN 
    Clients c ON c.client_id = r.client_id
INNER JOIN 
    Services s ON s.service_id = r.service_id
LEFT JOIN 
    Employees e ON e.employee_id = r.employee_id  
";
            if (MyData.role == "Мастер")
            {

                query += $@" where r.employee_id = '{MyData.employee_idLoaded}' ORDER BY 
  r.recording_datetime desc ";
            }
            else
            {
                query += @"ORDER BY 
  r.recording_datetime desc ";
            }
            
        UpdateGrid(query);
        }

        private void button_Click_3(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                using (MySqlCommand checkCmd = new MySqlCommand(
              $"SELECT status FROM Recording WHERE recording_id='{MyData.orders_id}'", con))
                {
                    string currentStatus = checkCmd.ExecuteScalar()?.ToString();


                    if (currentStatus != "На исполнении")
                    {
                        MessageBox.Show($"Нельзя отменить заказ со статусом: {currentStatus}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                        return;
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand($@"Update Recording Set status = 'Отменен' where recording_id='{MyData.orders_id}'", con))
                {
                    if (MessageBox.Show("Изменить статус на отменен?", "Изменение статуса", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Статус заказа изменен");
                        UpdateGrid(query);
                        button.IsEnabled = false;
                        button1.IsEnabled = false;

                    }

                }
            }

        }


    }
}