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

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Schedules.xaml
    /// </summary>
    public partial class Schedules : UserControl
    {
        public string query = @"SELECT 
    CONCAT_WS(' ', e.first_name, e.last_name, e.patronymic) AS 'ФИО',
    e.role AS 'Роль',
    DATE_FORMAT(es.dateSheldus, '%d.%m.%Y') AS 'Дата'
FROM 
    vkr.employee_schedules es
INNER JOIN 
    employees e ON e.employee_id = es.employee_id
WHERE 
    es.dateSheldus >= CURDATE()
ORDER BY 
    es.dateSheldus ASC;";
        public Schedules()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid(query);

            EditBtn.IsEnabled = true;

            switch (MyData.role)
            {
                case "Администратор":
                /*  AddBtn.Visibility = Visibility.Collapsed;
                    DelBtn.Visibility = Visibility.Collapsed;*/
                    EditBtn.Visibility = Visibility.Visible;
                    break;
                case "Мастер":
                    /*
                    AddBtn.Visibility = Visibility.Collapsed;
                    DelBtn.Visibility = Visibility.Collapsed;*/
                    EditBtn.Visibility = Visibility.Collapsed;
                    break;

                case "Директор":

                    break;

                case "Менеджер":
                    break;
            }

        }

        private void UpdateGrid(string query)
        {
            
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(viewBase.SqlConnection.connectionString))
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
        
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            Forms.SchedulesAdd schedulesEdit = new Forms.SchedulesAdd();
            MyData.idleTimer.Stop();
            schedulesEdit.ShowDialog();
            UpdateGrid(query);
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            EditBtn.IsEnabled = false;
        }

        private void DelBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.schedules_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
            }
            else
            {
                MyData.schedules_id = null;
            }
            */
        }

        private void datePicker1_CalendarOpened(object sender, RoutedEventArgs e)
        {
        
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
         
        }
    }
}
