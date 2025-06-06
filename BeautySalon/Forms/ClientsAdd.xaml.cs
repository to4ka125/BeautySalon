using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using BeautySalon.viewBase;
using System.Text.RegularExpressions;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for ClientsAdd.xaml
    /// </summary>
    public partial class ClientsAdd : Window
    {
        public ClientsAdd()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (first_name.Text.Length == 0 || 
                last_name.Text.Length == 0 ||
                phone.Text.Length == 0)
          
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }
            string name = first_name.Text;
            string lastName = last_name.Text;
            string phoneNumber = phone.Text;
            string emaill = email.Text;
            DateTime registrationDate = DateTime.Now;
            string formattedDate = registrationDate.ToString("yyyy-MM-dd HH:mm:ss"); 
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"insert into VKR.Clients (first_name,last_name, phone ,email,registration_date) 
                                                                                  values ('{name}','{lastName}','{phoneNumber}','{emaill}','{formattedDate}')", con);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Новый клиент добавлен");

                    first_name.Clear();
                    last_name.Clear();
                    phone.Clear();
                    email.Clear();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка - {ex.Message}");
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void first_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void last_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Яa-zA-Z\W]$")) { e.Handled = true; }
        }
        private void email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[@.]$")) { e.Handled = false; }
        }
    }
}