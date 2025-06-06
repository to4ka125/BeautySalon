using BeautySalon.viewBase;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using Word = Microsoft.Office.Interop.Word;

namespace BeautySalon.Forms
{
    /// <summary>
    /// Interaction logic for ViewBascket.xaml
    /// </summary>
    public partial class ViewBascket : Window
    {
        public ViewBascket()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Наименование");
            dt.Columns.Add("Кол-во");
            dt.Columns.Add("Цена");
            
                foreach(var item in Basket.basket)
                {
                    string idProduct = item.Key;
                    int quauty = item.Value;

                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                { con.Open();
                    MySqlCommand cmd = new MySqlCommand($@"select product_name, price
                                                                 from cosmetic_products where product_id ='{idProduct}'",con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader["product_name"].ToString();
                            string price = reader["price"].ToString();
                            dt.Rows.Add(name, quauty, price);
                        }
                    }
                    }
            }
            dataGrid.ItemsSource = dt.DefaultView;


        }
        private readonly string FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "check1.docx");
        public void GenerateCheck()
        {
            if (MessageBox.Show("Распечатать чек", "Чек", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                // Загрузка данных
                DataTable dt = new DataTable();
                dt.Columns.Add("Наименование");
                dt.Columns.Add("Кол-во");
                dt.Columns.Add("Цена");

                foreach (var item in Basket.basket)
                {
                    string idProduct = item.Key;
                    int quauty = item.Value;

                    using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand($@"select product_name, price
                                                                 from cosmetic_products where product_id ='{idProduct}'", con);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["product_name"].ToString();
                                string price = reader["price"].ToString();
                                dt.Rows.Add(name, quauty, price);
                            }
                        }
                    }
                }
              

                    // Работа с Word
                    var wordApp = new Microsoft.Office.Interop.Word.Application();
                    wordApp.Visible = true; // Делаем Word видимым
                    Microsoft.Office.Interop.Word.Document wordDocument = null;
                    string localCopyPath = null;

                    try
                    {
                        // Создаем копию шаблона
                        localCopyPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"check_copy_{Guid.NewGuid()}.docx");

                        int attempts = 0;
                        bool fileCopied = false;
                        while (attempts < 5 && !fileCopied)
                        {
                            try
                            {
                                File.Copy(FileName, localCopyPath, true);
                                fileCopied = true;
                            }
                            catch (IOException)
                            {
                                attempts++;
                                System.Threading.Thread.Sleep(500);
                            }
                        }

                        if (!fileCopied)
                            throw new Exception("Не удалось создать копию файла шаблона. Файл занят другим процессом.");

                        // Открываем документ
                        wordDocument = wordApp.Documents.Open(localCopyPath);

                        if (wordDocument.Tables.Count < 1)
                            throw new InvalidOperationException("Документ не содержит таблиц");

                    double sum = 0;
                        // Заполнение таблицы
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string name = dt.Rows[i]["Наименование"].ToString();
                            string quantity = dt.Rows[i]["Кол-во"].ToString();
                            double price = Convert.ToDouble(dt.Rows[i]["Цена"].ToString());

                            Word.Table table = wordDocument.Tables[1];
                            Word.Row newRow = table.Rows.Add();

                            newRow.Cells[1].Range.Text = name;
                            newRow.Cells[2].Range.Text = quantity;
                            newRow.Cells[3].Range.Text = price.ToString();
                        sum += price;
                        }

                        // Замена меток
                        ReplaceWordStub("{data}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), wordDocument);
                        ReplaceWordStub("{sum}", sum.ToString(), wordDocument);

                        // Сохраняем изменения
                        wordDocument.Save();

                        // Активируем окно Word
                        wordApp.Activate();

                        // Оставляем документ открытым для пользователя
                        // Теперь не закрываем документ и Word в finally блоке
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при генерации чека: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                        // В случае ошибки все равно нужно освободить ресурсы
                        if (wordDocument != null)
                        {
                            wordDocument.Close(false);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDocument);
                        }
                        if (wordApp != null)
                        {
                            wordApp.Quit();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                        }
                    }
                    finally
                    {
                        // Удаление временного файла шаблона
                        try
                        {
                            if (localCopyPath != null && File.Exists(localCopyPath))
                                File.Delete(localCopyPath);
                        }
                        catch { /* Игнорируем ошибки удаления */ }

                        // Освобождаем только COM-объекты документа (Word остается открытым)
                        if (wordDocument != null)
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordDocument);
                        }
                    }
                }
            }

        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                con.Open();
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                foreach(var item in Basket.basket)
                {
                    string id = item.Key;
                    int quatily = item.Value;

                    using (MySqlCommand cmd = 
                        new MySqlCommand($@"Insert into orders_cosmetic_products (product_id,quantity,order_date)
                                          values ('{id}','{quatily}','{date}')",con))
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Данные о покупке сохраненны");
                    }

                    using (MySqlCommand cmd = 
                        new MySqlCommand($@"Update  
                        cosmetic_products set quantity_in_stock=quantity_in_stock-'{quatily}' where product_id='{id}'",con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
                GenerateCheck();
            Close(); 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Basket.basket.Clear();
            Close();
        }
    }
}
