using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for EmployeesAdd.xaml
    /// </summary>
    public partial class EmployeesAdd : Window
    {
        public EmployeesAdd()
        {
            InitializeComponent();
        }
        private bool UserExists(string login)
        {
            MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString);
            con.Open();
            MySqlCommand cmd = new MySqlCommand($@"SELECT COUNT(*) FROM Employees WHERE login = '{login}'", con);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            con.Close();

            return count > 0;
        }
        public static string GetHashPass(string password)
        {
            // Преобразование строки пароля в массив байтов с использованием кодировки UTF-8
            byte[] bytesPass = Encoding.UTF8.GetBytes(password);

            // Создание экземпляра класса SHA256Managed для вычисления хешей
            SHA256Managed hashstring = new SHA256Managed();

            byte[] hash = hashstring.ComputeHash(bytesPass);

            string hashPasswd = string.Empty;

            foreach (byte x in hash)
            {
                // Форматирование байта в виде двухсимвольной шестнадцатеричной строки и добавление к результату
                hashPasswd += String.Format("{0:x2}", x);
            }

            hashstring.Dispose();

            return hashPasswd;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(first_name.Text) ||
                 string.IsNullOrWhiteSpace(last_name.Text) ||
                 string.IsNullOrWhiteSpace(patronymic.Text) ||
                 string.IsNullOrWhiteSpace(loginBox.Text) ||
                 string.IsNullOrWhiteSpace(passwordBox.Password) ||
         roleBox.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            string login = loginBox.Text;
            if (UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                string query;

                if (roleBox.Text == "Мастер")
                {
                    if (specBox.SelectedItem == null)
                    {
                        MessageBox.Show("Заполните все поля");
                        return;
                    }
                    query = $@"Insert into Employees (first_name,last_name,patronymic,login,password,role, idSpecialization) 
                                                              Values ('{first_name.Text}','{last_name.Text}',
                                                            '{patronymic.Text}','{loginBox.Text}','{GetHashPass(passwordBox.Password)}','{roleBox.Text}','{specBox.Text.Split(' ')[0]}')";
                }
                else
                {
                    query = $@"Insert into Employees (first_name,last_name,patronymic,login,password,role, idSpecialization) 
                                                              Values ('{first_name.Text}','{last_name.Text}',
                                                            '{patronymic.Text}','{loginBox.Text}','{GetHashPass(passwordBox.Password)}','{roleBox.Text}','0')";

                }
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Пользователь добавлен");

                    first_name.Clear();
                    last_name.Clear();
                    patronymic.Clear();
                    loginBox.Clear();
                    passwordBox.Clear();
                    roleBox.Text = "";
                }
            }
        }
        private void first_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void last_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void patronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я]$")) { e.Handled = true; }
        }
        private void passwordBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я]$")) { e.Handled = true; }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)passwordBox.Template.FindName("textBlock", passwordBox);
            if (passwordBox.Password.Length > 0)
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }
        private void roleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string role = (roleBox.SelectedItem as ComboBoxItem)?.Content?.ToString()
                                 ?? roleBox.SelectedValue?.ToString();

            if (role == "Мастер")
            {
                specBox.Visibility = Visibility.Visible;
                Height = 540;
            }
            else
            {
                specBox.Visibility = Visibility.Collapsed;
                Height = 480;
            }
        }
    }
}