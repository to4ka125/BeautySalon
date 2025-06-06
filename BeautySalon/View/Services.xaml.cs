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
using BeautySalon.viewBase;
using System.Text.RegularExpressions;
using BeautySalon.Forms;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : UserControl
    {

        string query = @"SELECT service_id,service_name AS 'Наименование услуги',description AS 'Описание',price AS 'Цена',duration AS 'Длительность' FROM Services where isDeleted='0'";
        public Services()
        {
            InitializeComponent();
        }
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.service_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
            else
            {
                MyData.orders_id = null;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
            UpdateGrid(query);

            if (MyData.role == "Администратор")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DellBtn.Visibility = Visibility.Collapsed;
                EditBtn.Visibility = Visibility.Collapsed;
            }
        }
        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText= searchBox.Text.Trim();
            query = $@"SELECT service_id,service_name AS 'Наименование услуги',description AS 'Описание',price AS 'Цена',duration AS 'Длительность' 
                FROM Services where isDeleted='0' and service_name LIKE '%{filterText}%'";

            UpdateGrid(query);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ServicesAdd ServicesAdd = new ServicesAdd();
            MyData.idleTimer.Stop();
            ServicesAdd.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ServicesEdit servicesEdit = new ServicesEdit();
            servicesEdit.ShowDialog();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                var result = MessageBox.Show("Вы действительно хотите удалить данную услугу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                    using (MySqlCommand cmd = new MySqlCommand($"Update Services set isDeleted='1' Where service_id ='{MyData.service_id}'",con))
                {
                    cmd.ExecuteNonQuery();
                        MessageBox.Show("Услуга удалена");
                        EditBtn.IsEnabled = false;
                        DellBtn.IsEnabled = false;
                    }
                UpdateGrid(query);
            }
        }   
        private void UpdateGrid(string query)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                   
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
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
        private void searchBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
    }
}
