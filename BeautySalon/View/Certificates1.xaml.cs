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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using BeautySalon.Model;

namespace BeautySalon.View
{
    /// <summary>
    /// Interaction logic for Certificates1.xaml
    /// </summary>
    public partial class Certificates1 : UserControl
    {
        public Certificates1()
        {
            InitializeComponent();
        }
        private readonly string FileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Template", "template.docx");
        Word.Document wordDocument;
        string id;
      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
            {
                var wordApp = new Word.Application();
                try
                {
                    wordDocument = wordApp.Documents.Open(FileName);
                }
                catch
                {
                    String path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "template.docx");
                    var wordDocument = wordApp.Documents.Open(FileName);
                }
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM VKR.Certificates where certificate_id = '{id}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    string certificate_number = dt.Rows[0].ItemArray[1].ToString();
                    string amount = dt.Rows[0].ItemArray[2].ToString();
                    string expiration_date = dt.Rows[0].ItemArray[3].ToString();

                    wordApp.Visible = false;

                    try
                    {
                     
                    // ReplaceWordStub("{certificate_number}", certificate_number, wordDocument);
                        ReplaceWordStub("{amount}", amount, wordDocument);
                    //  ReplaceWordStub("{expiration_date}", expiration_date, wordDocument);


                        wordApp.Visible = true;
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }                  
                }
            }
        }
        private void ReplaceWordStub(string stubToReplace, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Print.IsEnabled = false;
            try
            {
                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    con.Open();

                    string query = $@"Select certificate_id, amount As 'Сертификат на сумму' From Certificates";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        container.Children.Clear();
                        container.RowDefinitions.Clear();
                        container.ColumnDefinitions.Clear();
                        int rowCount = (int)Math.Ceiling((double)dt.Rows.Count / 3);

                        for (int i = 0; i < 3; i++)
                        {
                            container.ColumnDefinitions.Add(new ColumnDefinition
                            {
                                Width = new GridLength(1, GridUnitType.Star)
                            });
                        }
                        for (int i = 0; i < rowCount; i++)
                        {
                            container.RowDefinitions.Add(new RowDefinition
                            {
                                Height = GridLength.Auto
                            });
                        }
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var ser = new sertifacateBlock
                            {
                                Price = dt.Rows[i][1].ToString(),
                                ID= dt.Rows[i][0].ToString(),
                                 Margin = new Thickness(10),
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                            };
                            ser.MouseDown += Ser_MouseDown;
                            ser.MouseDoubleClick += Ser_MouseDoubleClick;
                            Grid.SetRow(ser, i / 3);
                            Grid.SetColumn(ser, i % 3);
                            container.Children.Add(ser);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

         
        }

        private void Ser_MouseDown(object sender, MouseButtonEventArgs e)
        {
         
        }

        private void Ser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is sertifacateBlock selectedSer)
            {
                id = selectedSer.id.Text;

                using (MySqlConnection con = new MySqlConnection(SqlConnection.connectionString))
                {
                    var wordApp = new Word.Application();
                    try
                    {
                        wordDocument = wordApp.Documents.Open(FileName);
                    }
                    catch
                    {
                        String path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "template.docx");
                        var wordDocument = wordApp.Documents.Open(FileName);
                    }
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand($"SELECT * FROM VKR.Certificates where certificate_id = '{id}'", con))
                    {
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        string certificate_number = dt.Rows[0].ItemArray[1].ToString();
                        string amount = dt.Rows[0].ItemArray[2].ToString();
                        string expiration_date = dt.Rows[0].ItemArray[3].ToString();
                        wordApp.Visible = false;
                        try
                        {

                            // ReplaceWordStub("{certificate_number}", certificate_number, wordDocument);
                            ReplaceWordStub("{amount}", amount, wordDocument);
                            //  ReplaceWordStub("{expiration_date}", expiration_date, wordDocument);


                            wordApp.Visible = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
