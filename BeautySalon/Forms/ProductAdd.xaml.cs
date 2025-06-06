using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for ProductAdd.xaml
    /// </summary>
    public partial class ProductAdd : Window
    {
        public ProductAdd()
        {
            InitializeComponent();
        }

        string fileName;

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == true)
                {
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                    long fileSizeInBytes = fileInfo.Length;
                    const long maxSizeInBytes = 2 * 1024 * 1024;

                    if (fileSizeInBytes > maxSizeInBytes)
                    {
                        MessageBox.Show("Размер файла превышает 2 МБ. Пожалуйста, выберите другой файл.", "Ошибка");
                    }
                    else
                    {
                        fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        string projectFolderPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string destinationFolderPath = System.IO.Path.Combine(projectFolderPath, "Image", "imageProduct");

                        if (!Directory.Exists(destinationFolderPath))
                        {
                            Directory.CreateDirectory(destinationFolderPath);
                        }
                        string destinationPath = System.IO.Path.Combine(destinationFolderPath, fileName);
                        File.Copy(openFileDialog.FileName, destinationPath, true);
                        image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                        MessageBox.Show($"Файл успешно сохранен в: {destinationPath}", "Успех");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (fileName == null && 
                DescriptionBox.Text == null && 
                TitleBox.Text == null && 
                PriceBox.Text == null && 
                TypeBox.Text == null && 
                QuantityBox.Text == null
                   )
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                string title = TitleBox.Text;
                string price = PriceBox.Text;
                string description = DescriptionBox.Text;
                string quantityBox = QuantityBox.Text;
                string type = TypeBox.Text;


                MySqlCommand cmd = new MySqlCommand($@"Insert into Cosmetic_Products (product_name,type,description,price,quantity_in_stock,image)
                                                       Values ('{title}','{type}','{description}','{price}','{quantityBox}','{fileName}')",con);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Продукт добавлен");

                DescriptionBox.Clear();
                TitleBox.Clear();
                PriceBox.Clear();
                TypeBox.Text = "";
                QuantityBox.Clear();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
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