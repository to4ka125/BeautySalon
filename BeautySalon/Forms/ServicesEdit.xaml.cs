using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for ServicesEdit.xaml
    /// </summary>
    public partial class ServicesEdit : Window
    {
        public ServicesEdit()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text == null || Description.Text == null || Price.Text == null || Duration.Text == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var result = MessageBox.Show("Вы действительно хотите изменить данные?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand($@"Update Services Set service_name = '{Name.Text}',description ='{Description.Text}',
                                                             price ='{Price.Text}',duration='{Duration.Text}' where service_id='{viewBase.MyData.service_id}'", con))
                    {
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Услуга изменена");
                        this.Close();
                        Name.Clear();
                        Description.Clear();
                        Price.Clear();
                        Duration.Clear();
                    }
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Duration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void Description_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
        }
        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($"Select * From Services where service_id ='{viewBase.MyData.service_id}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    Name.Text = dt.Rows[0].ItemArray[1].ToString();
                    Description.Text = dt.Rows[0].ItemArray[2].ToString();
                    Price.Text = dt.Rows[0].ItemArray[3].ToString();
                    Duration.Text = dt.Rows[0].ItemArray[4].ToString();
                }
            }
        }
    }
}