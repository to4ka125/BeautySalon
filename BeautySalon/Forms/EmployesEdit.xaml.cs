using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for EmployesEdit.xaml
    /// </summary>
    public partial class EmployesEdit : Window
    {
        public EmployesEdit()
        {
            InitializeComponent();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($"Select * From Employees Where employee_id = '{viewBase.MyData.employee_id}' ",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    first_name.Text = dt.Rows[0].ItemArray[1].ToString();
                    last_name.Text = dt.Rows[0].ItemArray[2].ToString();
                    patronymic.Text = dt.Rows[0].ItemArray[3].ToString();
                    loginBox.Text = dt.Rows[0].ItemArray[4].ToString();
                    roleBox.Text = dt.Rows[0].ItemArray[6].ToString();

                }
            }
        }
        public static string GetHashPass(string password)
        {

            byte[] bytesPass = Encoding.UTF8.GetBytes(password);

            SHA256Managed hashstring = new SHA256Managed();

            byte[] hash = hashstring.ComputeHash(bytesPass);

            string hashPasswd = string.Empty;

            foreach (byte x in hash)
            {
                hashPasswd += String.Format("{0:x2}", x);
            }

            hashstring.Dispose();

            return hashPasswd;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (first_name.Text == null || 
                last_name.Text == null || 
                patronymic.Text == null || 
                loginBox.Text == null
||              roleBox.SelectedItem == null)
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

                    string query;
                    if (passwordBox.Password == "")
                    {
                        query = $@"Update Employees set first_name ='{first_name.Text}',last_name='{last_name.Text}',patronymic='{patronymic.Text}',
login='{loginBox.Text}',role ='{roleBox.Text}' where employee_id = '{viewBase.MyData.employee_id}'";
                    }
                    else
                    {
                        query = $@"Update Employees set first_name ='{first_name.Text}',last_name='{last_name.Text}',patronymic='{patronymic.Text}',
login='{loginBox.Text}', password ='{GetHashPass(passwordBox.Password)}' ,role ='{roleBox.Text}' where employee_id = '{viewBase.MyData.employee_id}'";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись изменена");

                        first_name.Clear();
                        last_name.Clear();
                        patronymic.Clear();
                        loginBox.Clear();
                        passwordBox.Clear();
                        roleBox.Text = "";
                    }
                }
            }
        }
        private void last_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[a-zA-Z0-9\W]$")) { e.Handled = true; }
        }
        private void first_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
    }
}
