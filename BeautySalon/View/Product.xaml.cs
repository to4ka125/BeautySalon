using BeautySalon.Forms;
using BeautySalon.Model;
using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : UserControl
    {
        public string query = @"SELECT product_id , type As 'Тип' , product_name As 'Наименование продукта', concat_ws(' ',type,'|',product_name) As 'Наименование',
                                concat_ws(' ', price, 'р.') As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products where isDeleted='0'";
        public Product()
        {
            InitializeComponent();
        }
        private int currentPage = 1;
        private const int pageSize = 15;
        private int totalRecords;
        private void UpdatePaginationButtons(int totalPages)

        {
            PaginationBar.Children.Clear();
            for (int i = 0; i < (int)Math.Ceiling((double)totalRecords / pageSize); i++)
            {
                var paginationBtn = new Button
                {
                    Width = 30,
                    Height = 30,
                    Style = (Style)FindResource("BtnUC"),
                    Content = (i + 1).ToString(),
                    Margin = new Thickness(0, 0, 10, 0),
                    Name = $"Button_{i + 1}"
                };
                paginationBtn.Click += PaginationBtn_Click;
                PaginationBar.Children.Add(paginationBtn);
            }
        }
        private int GetTotalCount(List<string> filters)
        {

            string countQuery = @"SELECT COUNT(*) FROM Cosmetic_Products where isDeleted='0'";

            // Если есть условия фильтрации, добавляем их к запросу
            if (filters.Count > 0)
            {
                countQuery += string.Join(" AND ", filters);
            }

            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                using (MySqlCommand countCommand = new MySqlCommand(countQuery, con))
                {
                    totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());
                    return totalRecords;
                }
            }
        }
  
            // Начальное SQL-запрос
            private void filteringAndSorting()
            {
                // Начальное SQL-запрос
                query = @"SELECT product_id , type As 'Тип' , product_name As 'Наименование продукта', concat_ws(' ',type,'|',product_name) As 'Наименование',
                                concat_ws(' ', price, 'р.') As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products where isDeleted='0'";

                string sortOrder = "";
                List<string> filters = new List<string>(); // Список для хранения условий фильтрации

                // Сортировка по выбранному значению из ComboBox1
                if (ComboBox1.SelectedItem != null)
                {
                    string selectedSortValue = (ComboBox1.SelectedItem as ComboBoxItem)?.Content.ToString();
                    switch (selectedSortValue)
                    {
                        case "По Возврастанию":
                            sortOrder += " ORDER BY product_name ASC";
                            break;
                        case "По Убыванию":
                            sortOrder += " ORDER BY product_name DESC";
                            break;
                    }
                }

                // Фильтрация по типу, выбранному в ComboBox2
                if (ComboBox2.SelectedItem != null)
                {
                    string selectedTypeValue = (ComboBox2.SelectedItem as ComboBoxItem)?.Content.ToString();
                    filters.Add($"type = '{selectedTypeValue}'");
                }

                // Получение текста для поиска
                string filterText = searchBox.Text.Trim();

                // Добавляем фильтрацию по имени продукта
                if (!string.IsNullOrEmpty(filterText))
                {
                    filters.Add($"product_name LIKE '%{filterText}%'");
                }

                // Если есть условия фильтрации, добавляем их к запросу
                if (filters.Count > 0)
                {
                    query += string.Join(" AND ", filters);
                }

                // Получение общего количества записей для пагинации
                int totalCount = GetTotalCount(filters);

                // Определяем количество страниц
                int pageSize = 15; // Количество записей на странице
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Обновляем кнопки пагинации
                UpdatePaginationButtons(totalPages);

                // Добавляем сортировку к запросу
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    query += sortOrder;
                }

                // Обновляем DataGrid с новыми данными
                UpdateGrid(query, 1);
            }


            private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteringAndSorting();
        }
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filteringAndSorting();
        }
        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            filteringAndSorting();
        }
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                MyData.products_id = selectedRow[0].ToString();
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
            else
            {

                MyData.products_id = null;
            }*/
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
            UpdateGrid(query, currentPage);


            if (MyData.role =="Администратор")
            {
                AddBtn.Visibility = Visibility.Collapsed;
                DellBtn.Visibility = Visibility.Hidden;
               
            }
            for (int i = 0; i < (int)Math.Ceiling((double)totalRecords / pageSize); i++)
            {
                var paginationBtn = new Button
                {
                    Width = 30,
                    Height = 30,
                    Style = (Style)FindResource("BtnUC"),
                    Content = (i + 1).ToString(),
                    Margin = new Thickness(0, 0, 10, 0),
                    Name=$"Button_{i+1}"
                };
                paginationBtn.Click += PaginationBtn_Click;
                PaginationBar.Children.Add(paginationBtn);
            }
            }


        private  Button  selectedPaginationButton;
        private void PaginationBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (selectedPaginationButton != null)
            {
                selectedPaginationButton.Style = (Style)FindResource("BtnUC");
            }

            clickedButton.Style = (Style)FindResource("BtnStyleActive");
            selectedPaginationButton = clickedButton;

            currentPage = int.Parse(clickedButton.Content.ToString());
            UpdateGrid(query, currentPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ProductAdd ProductAdd = new ProductAdd();
            MyData.idleTimer.Stop();
            ProductAdd.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query, currentPage);
                EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            FormUtils.workTable.Opacity = 0.5;
            this.Opacity = 0.5;
            ProductEdit ProductEdit = new ProductEdit();
            MyData.idleTimer.Stop();
            ProductEdit.ShowDialog();
            MyData.idleTimer.Start();
            FormUtils.workTable.Opacity = 1;
            this.Opacity = 1;
            UpdateGrid(query,currentPage);
            EditBtn.IsEnabled = false;
            DellBtn.IsEnabled = false;
        }
        private void UpdateGrid(string query, int page)
        {
            ProductContainer.Children.Clear();
               query += $@" Limit {(page - 1) * pageSize}, {pageSize}";

            ImageSource image = null;

            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                string countQuery = "SELECT COUNT(*) FROM Cosmetic_Products";

                MySqlCommand countCommand = new MySqlCommand(countQuery, con);

                totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    try
                    {
                        string projectFolderPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string destinationFolderPath = System.IO.Path.Combine(projectFolderPath, "Image", "imageProduct");

                        string imagePath = destinationFolderPath + $"/{dt.Rows[i]["Image"]}";
                        image = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                    }
                    catch (Exception ex)
                    {

                    }

                    var dishes = new productBlock
                    {
                        ID = dt.Rows[i][0].ToString(),
                        Tytle = dt.Rows[i][3].ToString(),
                        Price = "Цена: " + dt.Rows[i][4].ToString(),
                        Stock = "На складе: " + dt.Rows[i][5].ToString(),
                        Source = image,
                        Margin = new Thickness(0, 10, 0, 10),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    dishes.MouseDown += Dishes_MouseDown;

                    ContextMenu cM = new ContextMenu();
                    MenuItem mi = new MenuItem();
                    mi.Header = "Посмотреть информацию";
                    mi.Click += MenuItem_Click;
                    MenuItem mBackcetAdd = new MenuItem();
                    mBackcetAdd.Header = "Добавить в корзину";
                    mBackcetAdd.Click += MBackcetAdd_Click;
                    
                    cM.Items.Add(mi);
                    cM.Items.Add(mBackcetAdd);
                    dishes.ContextMenu = cM;
                    ProductContainer.Children.Add(dishes);
                }

                /*
                query += $@" Limit {(page - 1) * pageSize}, {pageSize}";

                using (MySqlConnection connection = new MySqlConnection(viewBase.SqlConnection.connectionString))
                {
                    DataTable dataTable = new DataTable();
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                    connection.Open();
                    string countQuery = "SELECT COUNT(*) FROM Cosmetic_Products";

                    MySqlCommand countCommand = new MySqlCommand(countQuery, connection);

                    totalRecords = Convert.ToInt32(countCommand.ExecuteScalar());

                    try
                    {
                        dataAdapter.Fill(dataTable);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при извлечении данных: {ex.Message}");
                    }


                    DataTable dt1 = new DataTable();

                    dt1.Columns.Add("product_id",typeof(string));
                    dt1.Columns.Add("name_product",typeof(string));
                    dt1.Columns.Add("type",typeof(string));
                    dt1.Columns.Add("products_info",typeof(string));
                    dt1.Columns.Add("image_path", typeof(string)); // Добавляем колонку для пути к изображению

                    foreach (DataRow row in dataTable.Rows)
                    {
                        string idProduct = row["product_id"].ToString();
                        string nameProuctInSearch = row["Наименование продукта"].ToString();
                        string typeProductInFiltering = row["Тип"].ToString();

                        string nameProuct = row["Наименование"].ToString();

                        string price = row["Цена"].ToString();

                        string countProduct = row["Кол-во на складе"].ToString();

                        string productInfo = $"{nameProuct}\nНа складе: {countProduct} шт.\nЦена: {price}";

                        string imageName = row["image"].ToString(); // Получаем имя файла изображения

                        string projectFolderPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string destinationFolderPath = System.IO.Path.Combine(projectFolderPath, "Image", "imageProduct");

                        string imagePath = destinationFolderPath + $"/{imageName}";
                        dt1.Rows.Add(
                          idProduct,
                          nameProuctInSearch,
                          typeProductInFiltering,
                          productInfo,
                            imagePath
                        ) ;

                    }

                    dataGridView.AutoGenerateColumns = false;
                    dataGridView.Columns.Clear();

                    dataGridView.Columns.Add(new DataGridTextColumn()
                    {
                        Header = "ID",
                        Binding = new Binding("product_id"),
                        Visibility = Visibility.Collapsed
                    });

                    // Добавляем колонку с изображением




                    // Добавляем колонку с информацией
                    var infoColumn = new DataGridTextColumn()
                    {
                        Header = "Информация о продукте",
                        Binding = new Binding("products_info"),
                        Width = new DataGridLength(1, DataGridLengthUnitType.Star)
                    };
                    var imageColumn = new DataGridTemplateColumn()
                    {
                        Header = "Изображение",
                        Width = new DataGridLength(200)
                    };

                    var imageFactory = new FrameworkElementFactory(typeof(Image));
                    imageFactory.SetValue(Image.WidthProperty, 100.0);
                    imageFactory.SetValue(Image.HeightProperty, 100.0);
                    imageFactory.SetValue(Image.StretchProperty, Stretch.UniformToFill);
                    imageFactory.SetBinding(Image.SourceProperty, new Binding("image_path"));

                    imageColumn.CellTemplate = new DataTemplate() { VisualTree = imageFactory };


                    var textBlockStyle = new Style(typeof(TextBlock));
                    textBlockStyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                    infoColumn.ElementStyle = textBlockStyle;

                    dataGridView.Columns.Add(infoColumn);
                    dataGridView.Columns.Add(imageColumn);
                    // Устанавливаем источник данных
                    dataGridView.ItemsSource = dt1.DefaultView;

                    // Настройка высоты строк
                    dataGridView.RowHeight = Double.NaN; // Автоматическая высота
                    dataGridView.EnableRowVirtualization = false;
                    dataGridView.EnableColumnVirtualization = false;


                    // Стиль для строк
                    var rowStyle = new Style(typeof(DataGridRow));
                    rowStyle.Setters.Add(new Setter(HeightProperty, Double.NaN));
                    rowStyle.Setters.Add(new Setter(MinHeightProperty, 60.0));
                    dataGridView.RowStyle = rowStyle; dataGridView.RowHeight = Double.NaN; // Автоматическая высота
                    dataGridView.EnableRowVirtualization = false;
                    dataGridView.EnableColumnVirtualization = false;


                    // Стиль для строк

                }
             */
            }
        }

        private void MBackcetAdd_Click(object sender, RoutedEventArgs e)
        {
            if (MyData.products_id !=null)
            {
                Basket.Instance.AddToBasket(MyData.products_id);

                foreach (var item in Basket.basket)
                {
                    int quantityInBasket = item.Value;
                    using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
                    {
                        con.Open();
                        List<double> availableQuantities = new List<double>();
                        using (MySqlCommand cmd = new MySqlCommand(@"SELECT quantity_in_stock FROM vkr.cosmetic_products;", con))
                        {
                            using (MySqlDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    availableQuantities.Add(dr.GetDouble(0));
                                }
                            }
                        }


                        availableQuantities[int.Parse(item.Key) - 1] -= quantityInBasket;
                        if (availableQuantities[int.Parse(item.Key) - 1] < 0)
                        {
                            Basket.Instance.DellIngredients(item.Key);
                            MessageBox.Show("На складе недостаточно продуктов", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                }
            }
                  
                countBascket.Text = Basket.basket.Sum(item => item.Value).ToString();
           }
        
   
        private void Dishes_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is productBlock pB)
            {
                MyData.products_id = pB.id.Text;
                EditBtn.IsEnabled = true;
                DellBtn.IsEnabled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(viewBase.SqlConnection.connectionString))
            {
                con.Open();

                var result = MessageBox.Show("Вы действительно хотите удалить данный товар?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                    using (MySqlCommand cmd = new MySqlCommand($"Update cosmetic_products set isDeleted = '1' WHERE product_id = '{MyData.products_id}'", con))
                {
                    cmd.ExecuteNonQuery();
              
                }
                MessageBox.Show("Товар удалён");
                EditBtn.IsEnabled = false;
                DellBtn.IsEnabled = false;
                UpdateGrid(query, currentPage);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            {
                ComboBox1.SelectedItem = null;
                ComboBox2.SelectedItem = null;
                searchBox.Text = string.Empty;
                query = $@" SELECT product_id , type As 'Тип' , product_name As 'Наименование продукта', concat_ws(' ', type, '|', product_name) As 'Наименование',
                                concat_ws(' ', price, 'р.') As 'Цена', 
                                quantity_in_stock As 'Кол-во на складе', image From Cosmetic_Products where isDeleted='0'";
                currentPage = 1;
                UpdateGrid(query, currentPage);
                EditBtn.IsEnabled = false;
                DellBtn.IsEnabled = false;
            }
        }

        private void searchBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            int maxPage = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Проверяем, что текущая страница меньше максимальной
            if (currentPage < maxPage)
            {
                foreach (Button items in PaginationBar.Children)
                {
                    string[] butonName = items.Name.Split('_');

                    if (butonName[1] == (currentPage + 1).ToString())
                    {
                        items.Style = (Style)FindResource("BtnUCActive");
                        selectedPaginationButton = items;
                    }
                    if (butonName[1] == currentPage.ToString())
                    {
                        items.Style = (Style)FindResource("BtnUC");
                    }
                }

                currentPage += 1;
                UpdateGrid(query, currentPage);
            }
        }

        private void Btn_Click_2(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                foreach (Button items in PaginationBar.Children)
                {
                    string[] butonName = items.Name.Split('_');

                    if (butonName[1] == currentPage.ToString())
                    {
                        items.Style = (Style)FindResource("BtnUC");
                        selectedPaginationButton = items;
                    }
                    if (butonName[1] == (currentPage - 1).ToString())
                    {
                        items.Style = (Style)FindResource("BtnUCActive");
                    }
                }
                currentPage -= 1;
                UpdateGrid(query, currentPage);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProductEdit pE = new ProductEdit();
            MyData.view = true;
            pE.ShowDialog();
            MyData.view = false;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Basket.basket.Clear();
            countBascket.Text = Basket.basket.Sum(item => item.Value).ToString();
        }

        private void bascketBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewBascket Vb = new ViewBascket();
            Vb.ShowDialog();
            UpdateGrid(query, currentPage);
        }
    }
}