using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ServicesAdd.xaml
    /// </summary>
    public partial class ServicesAdd : Window
    {
        public ServicesAdd()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text==null || 
                Description.Text ==null || 
                Price.Text==null || 
                Duration.Text ==null )
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($@"Insert into service_id (service_name,description,price,duration) 
                                                              Values ('{Name.Text}','{Description.Text}','{Price.Text}','{Duration.Text}')", con))
                {
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Услуга добавлена");

                    Name.Clear();
                    Description.Clear();
                    Price.Clear();
                    Duration.Clear();
                   
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
        }
        private void Description_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
        }
        private void Price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void Duration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}