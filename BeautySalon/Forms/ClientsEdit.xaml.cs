using BeautySalon.viewBase;
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
    /// Interaction logic for ClientsEdit.xaml
    /// </summary>
    public partial class ClientsEdit : Window
    {
        public ClientsEdit()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(first_name.Text) ||
                string.IsNullOrWhiteSpace(last_name.Text) ||
                string.IsNullOrWhiteSpace(phone.Text))
                {
                MessageBox.Show("Заполните обязательны поля");
                return;
            }
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                var result = MessageBox.Show("Вы действительно хотите изменить данные?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($@"Update Clients Set first_name ='{first_name.Text}',
                                                             last_name='{last_name.Text}',phone='{phone.Text}',email='{email.Text}' where client_id ='{MyData.clients_id}'", con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Данный о пользователь изменен");

                        first_name.Clear();
                        last_name.Clear();
                        phone.Clear();
                        email.Clear();
                    }
                }
            }
        }
            private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyData.view)
            {
                tytle.Text = "Просмотр информации о клиенте";
                addBtn.Visibility = Visibility.Hidden;

                first_name.IsReadOnly = true;
                last_name.IsReadOnly = true;
                phone.IsReadOnly = true;
                email.IsReadOnly = true;
            }
          
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($@"Select client_id,first_name,last_name,phone,email From Clients
                                                              Where client_id='{MyData.clients_id}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    first_name.Text = dt.Rows[0].ItemArray[1].ToString();
                    last_name.Text = dt.Rows[0].ItemArray[2].ToString();
                    phone.Text = dt.Rows[0].ItemArray[3].ToString();
                    email.Text = dt.Rows[0].ItemArray[4].ToString();
                        
                }
            }
        }
        private void phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Яa-zA-Z\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[+]$")) { e.Handled = false; }
        }
        private void email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[@.]$")) { e.Handled = false; }
        }
        private void last_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }

        }
        private void first_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
    }
}