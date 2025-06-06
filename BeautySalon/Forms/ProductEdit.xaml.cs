using System;
using System.Collections.Generic;
using System.Data;
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
using BeautySalon.viewBase;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for ProductEdit.xaml
    /// </summary>
    public partial class ProductEdit : Window
    {

        string FileName=null;
        public ProductEdit()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (FileName == null && DescriptionBox.Text == null && TitleBox.Text == null && PriceBox.Text == null
                  && TypeBox.Text == null && QuantityBox.Text == null
                  )
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var result = MessageBox.Show("Вы действительно хотите изменить данные?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand($@"Update Cosmetic_Products
                                                       Set product_name = '{TitleBox.Text}',type = '{TypeBox.Text}',description ='{DescriptionBox.Text}',
                                                       price ='{ PriceBox.Text.Replace(',', '.')}',quantity_in_stock='{QuantityBox.Text}',image ='{FileName}' where product_id = '{viewBase.MyData.products_id}'", con);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Запись изменена");

                    DescriptionBox.Clear();
                    TitleBox.Clear();
                    PriceBox.Clear();
                    TypeBox.Text = "";
                    QuantityBox.Clear();
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyData.role == "Администратор")
            {
                TitleBox.IsReadOnly = true;
                TypeBox.IsEnabled = false;
                DescriptionBox.IsReadOnly = true;
                PriceBox.IsReadOnly = true;
            }

            if (MyData.view)
            {
                TitleBox.IsReadOnly = true;
                TypeBox.IsEnabled = false;
                DescriptionBox.IsReadOnly = true;
                PriceBox.IsReadOnly = true;
                QuantityBox.IsReadOnly = true;
                btn1.Visibility = Visibility.Hidden;
                btn2.Visibility = Visibility.Hidden;
            }
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($"Select * From Cosmetic_Products Where product_id = '{MyData.products_id}'",con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                TitleBox.Text = dt.Rows[0].ItemArray[1].ToString();
                TypeBox.Text = dt.Rows[0].ItemArray[2].ToString();
                DescriptionBox.Text = dt.Rows[0].ItemArray[3].ToString();
                PriceBox.Text = dt.Rows[0].ItemArray[4].ToString();
                QuantityBox.Text = dt.Rows[0].ItemArray[5].ToString();




                FileName = dt.Rows[0].ItemArray[6].ToString();

                try
                {
                    if (FileName != null || FileName !="")
                    {
                        image.Source = new BitmapImage(new Uri($"/Image/imageProduct/{dt.Rows[0]["image"]}", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        image.Source = null;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
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
                    const long maxSizeInBytes = 2 * 1024 * 1024;  // Определение максимального размера файла (2 МБ)

                    if (fileSizeInBytes > maxSizeInBytes)
                    {
                        MessageBox.Show("Размер файла превышает 2 МБ. Пожалуйста, выберите другой файл.", "Ошибка");
                    }
                    else
                    {
                        FileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                        string projectFolderPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string destinationFolderPath = System.IO.Path.Combine(projectFolderPath, "Image", "imageProduct");

                        if (!Directory.Exists(destinationFolderPath))
                        {
                            Directory.CreateDirectory(destinationFolderPath);
                        }
                        string destinationPath = System.IO.Path.Combine(destinationFolderPath, FileName);
                        File.Copy(openFileDialog.FileName, destinationPath, true);
                        image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                        MessageBox.Show($"Файл успешно сохранен в: {destinationPath}", "Успех");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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