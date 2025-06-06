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
using System.Data;
using System.Text.RegularExpressions;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for MaterialEdit.xaml
    /// </summary>
    public partial class MaterialEdit : Window
    {
        public MaterialEdit()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TitleBox.Text.Length == 0 || 
                TypeBox.Text.Length == 0 ||
                QuantityBox.Text.Length == 0 || 
                DescriptionBox.Text.Length == 0 ||
                PriceBox.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            string title = TitleBox.Text;
            string type = TypeBox.Text;
            string quantity_in_stock = QuantityBox.Text;
            string description = DescriptionBox.Text;
            string price = PriceBox.Text;

            var result = MessageBox.Show("Вы действительно хотите изменить данные?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {

                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"Update Materials
                                                       Set material_name = '{TitleBox.Text}',type = '{TypeBox.Text}',description ='{DescriptionBox.Text}',
                                                       quantity_in_stock='{QuantityBox.Text}',  price ='{ PriceBox.Text.Replace(',', '.')}' where material_id = '{viewBase.MyData.id}'", con);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Материал изменен");

                    TitleBox.Clear();
                    TypeBox.Text = "";
                    QuantityBox.Clear();
                    DescriptionBox.Clear();
                    PriceBox.Clear();
                }
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand($"Select * From Materials Where material_id ='{MyData.id}'",con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                TitleBox.Text = dt.Rows[0].ItemArray[1].ToString();
                TypeBox.Text = dt.Rows[0].ItemArray[2].ToString();
                DescriptionBox.Text = dt.Rows[0].ItemArray[3].ToString();
                QuantityBox.Text = dt.Rows[0].ItemArray[4].ToString();
                PriceBox.Text = dt.Rows[0].ItemArray[5].ToString();
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
    }
}