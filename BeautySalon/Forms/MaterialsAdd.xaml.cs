using MySql.Data.MySqlClient;
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
using BeautySalon.viewBase;
using System.Text.RegularExpressions;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for MaterialsAdd.xaml
    /// </summary>
    public partial class MaterialsAdd : Window
    {
        public MaterialsAdd()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (TitleBox.Text.Length == 0 || 
                TypeBox.Text.Length == 0 ||
                QuantityBox.Text.Length == 0 || 
                DescriptionBox.Text.Length == 0 ||
                PriceBox.Text.Length == 0)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }
            string title = TitleBox.Text;
            string type = TypeBox.Text;
            string quantity_in_stock = QuantityBox.Text;
            string description = DescriptionBox.Text;
            string price = PriceBox.Text;

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($@"insert into VKR.Materials (material_name,`type`,description,quantity_in_stock,price) 
                                                                                  values ('{title}','{type}','{description}','{quantity_in_stock}','{price}')", con);
               
               cmd.ExecuteNonQuery();
               MessageBox.Show("Новый материал добавлен");

                TitleBox.Clear();
                TypeBox.Text = "";
                QuantityBox.Clear();
                DescriptionBox.Clear();
                PriceBox.Clear();
            }
        }
        private void TitleBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
        }
        private void QuantityBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
        }
        private void DescriptionBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[\W]$")) { e.Handled = true; }
        }
        private void DescriptionBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {

        }
    }
}